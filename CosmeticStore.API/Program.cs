using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;
using CosmeticStore.Core.Events;
using CosmeticStore.Infrastructure.Events;
using CosmeticStore.Infrastructure.Gateways;
using CosmeticStore.Infrastructure.Handlers;
using CosmeticStore.Infrastructure.Handlers.Notifications;
using CosmeticStore.Infrastructure.Repositories;
using CosmeticStore.Infrastructure.Services;
using CosmeticStore.Infrastructure.Strategies;

var builder = WebApplication.CreateBuilder(args);

// ==========================================
// 1. CẤU HÌNH DATABASE (Entity Framework Core)
// ==========================================
builder.Services.AddDbContext<StoreDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("CosmeticStore.Infrastructure")
    )
);

// ==========================================
// 2. CẤU HÌNH JWT AUTHENTICATION
// ==========================================
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? "CosmeticStore_SuperSecretKey_12345678901234567890";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "CosmeticStore.API";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "CosmeticStore.Client";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
        ClockSkew = TimeSpan.Zero // Không cho phép sai lệch thời gian
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ==========================================
// 3. ĐĂNG KÝ DEPENDENCY INJECTION (DI)
// Áp dụng tính TRỪU TƯỢNG (Abstraction)
// Controller chỉ biết Interface, không biết Implementation cụ thể
// ==========================================

// Authentication Service: Đăng ký, Đăng nhập, JWT Token
builder.Services.AddScoped<IAuthService, AuthService>();

// Repository Pattern: Đăng ký Generic Repository cho các Entity chung
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// Repository Pattern: Đăng ký Product Repository với các method đặc thù
// - GetBySkinTypeAsync: Lọc sản phẩm theo loại da
// - GetExpiringSoonAsync: Lọc sản phẩm cận hạn
// - GetFlashSaleProductsAsync: Lấy sản phẩm Flash Sale
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Repository Pattern: Đăng ký Order Repository
// - GetWithItemsAsync: Lấy đơn hàng kèm chi tiết
// - GetByUserIdAsync: Lấy đơn hàng theo User
// - GetByStatusAsync: Lấy đơn hàng theo trạng thái
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Strategy Pattern: Đăng ký chiến lược tính giá mặc định
// Có thể thay đổi sang VipPricingStrategy hoặc SalePricingStrategy tùy logic
builder.Services.AddScoped<IPricingStrategy, StandardPricingStrategy>();

// Đăng ký tất cả Strategy để sử dụng khi cần
builder.Services.AddScoped<VipPricingStrategy>();
builder.Services.AddScoped<StandardPricingStrategy>();
builder.Services.AddScoped<SalePricingStrategy>();
builder.Services.AddScoped<SkinTypePricingStrategy>();

// Pricing Service: Orchestrator cho Strategy + Decorator Pattern
// - Tự động chọn Strategy phù hợp (VIP, SkinType, Standard)
// - Tự động wrap Decorator cần thiết (Expiry, FlashSale, Coupon)
builder.Services.AddScoped<IPricingService, PricingService>();

// ==========================================
// GIAI ĐOẠN 5: AI & Nâng cao
// ==========================================
// AI Skin Quiz Service:
// - Phân tích câu trả lời → Xác định loại da
// - Cập nhật User.SkinType → User hưởng SkinTypePricingStrategy
// - Gợi ý sản phẩm phù hợp loại da
builder.Services.AddScoped<ISkinQuizService, SkinQuizService>();

// Factory Pattern: Đăng ký Payment Factory (Legacy)
builder.Services.AddScoped<PaymentFactory>();

// Factory Pattern: Đăng ký Payment Gateway Factory
// - CreateGateway("MOMO") → MomoGateway
// - CreateGateway("ZALOPAY") → ZaloPayGateway
// - CreateGateway("VNPAY") → VNPayGateway
// - CreateGateway("COD") → CODGateway
builder.Services.AddScoped<PaymentGatewayFactory>();

// Singleton Pattern: Đăng ký Logger - Toàn hệ thống dùng chung 1 instance
// - Ghi log API request, lỗi thanh toán, tạo/hủy đơn
builder.Services.AddSingleton<IAppLogger, AppLogger>();

// ==========================================
// SINGLETON PATTERN: System Logger (Nâng cao)
// ==========================================
// - Chỉ có 1 instance duy nhất trong toàn bộ vòng đời ứng dụng
// - Ghi log vào File (logs/system-yyyy-MM-dd.log)
// - Ghi log vào Database (bảng SystemLogs)
// - Thread-safe với ConcurrentQueue
// - Batch writing để tối ưu hiệu năng
builder.Services.AddSingleton<ISystemLogger, SystemLogger>();

// ==========================================
// COMMAND PATTERN: Đăng ký Command Handlers
// Mỗi Command có 1 Handler tương ứng
// Single Responsibility: Mỗi handler chỉ làm 1 việc
// ==========================================

// CreateOrderCommandHandler: Validate → Tính giá → Lưu DB → Log
builder.Services.AddScoped<ICommandHandler<CreateOrderCommand, CreateOrderResult>, CreateOrderCommandHandler>();

// CancelOrderCommandHandler: Validate → Hủy → Hoàn tồn kho → Log
builder.Services.AddScoped<ICommandHandler<CancelOrderCommand, CancelOrderResult>, CancelOrderCommandHandler>();

// ConfirmOrderCommandHandler: Validate → Xác nhận → Update shipping → Log
builder.Services.AddScoped<ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult>, ConfirmOrderCommandHandler>();

// PayOrderCommandHandler: Validate → Factory tạo Payment Service → Process → Log
builder.Services.AddScoped<ICommandHandler<PayOrderCommand, PayOrderResult>, PayOrderCommandHandler>();

// ==========================================
// OBSERVER PATTERN: Domain Events & Handlers
// ==========================================
// - Domain Event Dispatcher: Phân phối events đến handlers
// - Notification Service: Gửi Email/SMS/Push/Admin Alert
// - Event Handlers: Lắng nghe và xử lý events

// Core Services
builder.Services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
builder.Services.AddScoped<INotificationService, NotificationService>();

// Email Notification Handlers
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedEmailHandler>();
builder.Services.AddScoped<IDomainEventHandler<PaymentSuccessEvent>, PaymentSuccessEmailHandler>();
builder.Services.AddScoped<IDomainEventHandler<OrderCancelledEvent>, OrderCancelledEmailHandler>();

// SMS Notification Handlers
builder.Services.AddScoped<IDomainEventHandler<OrderCreatedEvent>, OrderCreatedSmsHandler>();
builder.Services.AddScoped<IDomainEventHandler<OrderConfirmedEvent>, OrderConfirmedSmsHandler>();
builder.Services.AddScoped<IDomainEventHandler<PaymentFailedEvent>, PaymentFailedSmsHandler>();
builder.Services.AddScoped<IDomainEventHandler<OrderDeliveredEvent>, OrderDeliveredSmsHandler>();

// Admin Alert Handlers
builder.Services.AddScoped<IDomainEventHandler<ReviewCreatedEvent>, ReviewCreatedAdminHandler>();
builder.Services.AddScoped<IDomainEventHandler<ReviewReportedEvent>, ReviewReportedAdminHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductExpiringSoonEvent>, ProductExpiringSoonAdminHandler>();
builder.Services.AddScoped<IDomainEventHandler<ProductLowStockEvent>, ProductLowStockAdminHandler>();
builder.Services.AddScoped<IDomainEventHandler<PaymentFailedEvent>, PaymentFailedAdminHandler>();
builder.Services.AddScoped<IDomainEventHandler<FlashSaleActivatedEvent>, FlashSaleNotificationHandler>();

// ==========================================
// 4. CẤU HÌNH API & SWAGGER
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Swagger với JWT Authentication
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CosmeticStore API",
        Version = "v1",
        Description = "API cho hệ thống bán mỹ phẩm - Đồ án OOP với Design Patterns",
        Contact = new OpenApiContact
        {
            Name = "CosmeticStore Team",
            Email = "support@cosmeticstore.com"
        }
    });

    // Cấu hình JWT Bearer cho Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT token vào đây. Ví dụ: eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ==========================================
// 5. MIDDLEWARE PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    // Swagger UI: http://localhost:5xxx/swagger
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "CosmeticStore API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

// QUAN TRỌNG: Authentication phải trước Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;
using CosmeticStore.Infrastructure.Gateways;
using CosmeticStore.Infrastructure.Handlers;
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
// 2. ĐĂNG KÝ DEPENDENCY INJECTION (DI)
// Áp dụng tính TRỪU TƯỢNG (Abstraction)
// Controller chỉ biết Interface, không biết Implementation cụ thể
// ==========================================

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
// 3. CẤU HÌNH API & SWAGGER
// ==========================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// ==========================================
// 4. MIDDLEWARE PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    // Swagger UI: http://localhost:5xxx/swagger
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "CosmeticStore API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

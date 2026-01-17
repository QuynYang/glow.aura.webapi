using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;
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

// Strategy Pattern: Đăng ký chiến lược tính giá mặc định
// Có thể thay đổi sang VipPricingStrategy hoặc SalePricingStrategy tùy logic
builder.Services.AddScoped<IPricingStrategy, StandardPricingStrategy>();

// Đăng ký tất cả Strategy để sử dụng khi cần
builder.Services.AddScoped<VipPricingStrategy>();
builder.Services.AddScoped<StandardPricingStrategy>();
builder.Services.AddScoped<SalePricingStrategy>();

// Factory Pattern: Đăng ký Payment Factory
builder.Services.AddScoped<PaymentFactory>();

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

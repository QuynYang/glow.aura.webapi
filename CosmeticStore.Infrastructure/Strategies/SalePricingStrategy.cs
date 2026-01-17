using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá khuyến mãi - Giảm theo phần trăm tùy chỉnh
/// 
/// Áp dụng tính ĐA HÌNH (Polymorphism):
/// - Cùng interface IPricingStrategy nhưng hành vi khác nhau
/// - SalePricingStrategy áp dụng % giảm giá cố định cho tất cả
/// 
/// Sử dụng cho:
/// - Chương trình khuyến mãi theo mùa
/// - Flash Sale toàn bộ sản phẩm
/// - Event đặc biệt (Black Friday, 11/11, ...)
/// </summary>
public class SalePricingStrategy : IPricingStrategy
{
    private readonly decimal _discountPercent;

    public string StrategyName => "Sale Pricing";
    
    public string Description => $"Giảm giá chương trình khuyến mãi: {_discountPercent * 100}%";

    /// <summary>
    /// Khởi tạo với phần trăm giảm giá tùy chỉnh
    /// </summary>
    /// <param name="discountPercent">Phần trăm giảm (0.0 - 1.0), mặc định 20%</param>
    public SalePricingStrategy(decimal discountPercent = 0.20m)
    {
        if (discountPercent < 0 || discountPercent > 1)
            throw new ArgumentException("Phần trăm giảm giá phải từ 0 đến 1", nameof(discountPercent));
        
        _discountPercent = discountPercent;
    }

    /// <summary>
    /// Tính giá sau khi áp dụng khuyến mãi
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng (không sử dụng - áp dụng cho tất cả)</param>
    /// <returns>Giá sau khi giảm</returns>
    public decimal CalculatePrice(Product product, User? user)
    {
        return product.Price * (1 - _discountPercent);
    }

    /// <summary>
    /// Lấy phần trăm giảm giá
    /// </summary>
    public decimal GetDiscountPercent(Product product, User? user)
    {
        return _discountPercent;
    }
}

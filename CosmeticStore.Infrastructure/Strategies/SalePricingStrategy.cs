using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá khuyến mãi - Giảm theo phần trăm tùy chỉnh
/// Áp dụng tính ĐA HÌNH (Polymorphism): Cùng interface nhưng hành vi khác nhau
/// </summary>
public class SalePricingStrategy : IPricingStrategy
{
    private readonly decimal _discountPercentage;

    public string StrategyName => $"Sale Pricing - Giảm {_discountPercentage * 100}%";

    /// <summary>
    /// Khởi tạo với phần trăm giảm giá tùy chỉnh
    /// </summary>
    /// <param name="discountPercentage">Phần trăm giảm (0.0 - 1.0)</param>
    public SalePricingStrategy(decimal discountPercentage = 0.20m)
    {
        if (discountPercentage < 0 || discountPercentage > 1)
            throw new ArgumentException("Phần trăm giảm giá phải từ 0 đến 1", nameof(discountPercentage));
        
        _discountPercentage = discountPercentage;
    }

    /// <summary>
    /// Tính giá sau khi áp dụng khuyến mãi
    /// </summary>
    public decimal CalculatePrice(decimal originalPrice)
    {
        return originalPrice * (1 - _discountPercentage);
    }
}



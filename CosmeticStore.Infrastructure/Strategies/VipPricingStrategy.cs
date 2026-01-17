using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá cho khách hàng VIP - Giảm 10%
/// Áp dụng tính ĐA HÌNH (Polymorphism): Cùng interface nhưng hành vi khác nhau
/// </summary>
public class VipPricingStrategy : IPricingStrategy
{
    private const decimal VIP_DISCOUNT = 0.10m; // Giảm 10%

    public string StrategyName => "VIP Pricing - Giảm 10%";

    /// <summary>
    /// Tính giá sau khi giảm 10% cho khách VIP
    /// </summary>
    public decimal CalculatePrice(decimal originalPrice)
    {
        return originalPrice * (1 - VIP_DISCOUNT);
    }
}



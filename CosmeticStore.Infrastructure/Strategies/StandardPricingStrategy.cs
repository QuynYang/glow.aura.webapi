using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá thông thường - Giữ nguyên giá
/// Áp dụng tính ĐA HÌNH (Polymorphism): Cùng interface nhưng hành vi khác nhau
/// </summary>
public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard Pricing - Giá gốc";

    /// <summary>
    /// Giữ nguyên giá gốc cho khách hàng thông thường
    /// </summary>
    public decimal CalculatePrice(decimal originalPrice)
    {
        return originalPrice;
    }
}



namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Strategy Pattern cho việc tính giá - Áp dụng tính ĐA HÌNH (Polymorphism)
/// 
/// Cùng một method CalculatePrice() nhưng các class implement khác nhau
/// sẽ có cách tính khác nhau:
/// - VipPricingStrategy: Giảm 10% cho khách VIP
/// - StandardPricingStrategy: Giữ nguyên giá
/// - SalePricingStrategy: Giảm giá theo chương trình khuyến mãi
/// 
/// Lợi ích của Strategy Pattern:
/// - Dễ dàng thêm chiến lược tính giá mới mà không sửa code cũ (Open/Closed Principle)
/// - Có thể thay đổi chiến lược tính giá linh hoạt trong runtime
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Tính giá sau khi áp dụng chiến lược
    /// </summary>
    /// <param name="originalPrice">Giá gốc sản phẩm</param>
    /// <returns>Giá sau khi áp dụng chiến lược</returns>
    decimal CalculatePrice(decimal originalPrice);

    /// <summary>
    /// Tên chiến lược tính giá
    /// </summary>
    string StrategyName { get; }
}



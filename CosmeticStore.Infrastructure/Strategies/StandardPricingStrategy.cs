using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá thông thường - Giữ nguyên giá gốc
/// 
/// Áp dụng tính ĐA HÌNH (Polymorphism):
/// - Cùng interface IPricingStrategy nhưng hành vi khác nhau
/// - StandardPricingStrategy không áp dụng bất kỳ giảm giá nào
/// 
/// Sử dụng cho:
/// - Khách vãng lai (chưa đăng nhập)
/// - Khách hàng thường (VipLevel.None)
/// - Trường hợp mặc định
/// </summary>
public class StandardPricingStrategy : IPricingStrategy
{
    public string StrategyName => "Standard Pricing";
    
    public string Description => "Giá gốc - Không áp dụng giảm giá";

    /// <summary>
    /// Giữ nguyên giá gốc cho khách hàng thông thường
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng (không sử dụng trong strategy này)</param>
    /// <returns>Giá gốc của sản phẩm</returns>
    public decimal CalculatePrice(Product product, User? user)
    {
        return product.Price;
    }

    /// <summary>
    /// Không có giảm giá
    /// </summary>
    public decimal GetDiscountPercent(Product product, User? user)
    {
        return 0m;
    }
}

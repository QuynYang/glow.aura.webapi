using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá cho khách hàng VIP - Giảm theo cấp độ VIP
/// 
/// Áp dụng tính ĐA HÌNH (Polymorphism):
/// - Cùng interface IPricingStrategy nhưng hành vi khác nhau
/// - VipPricingStrategy giảm giá dựa trên User.VipLevel
/// 
/// Bảng giảm giá theo VipLevel:
/// - None: 0% (không giảm)
/// - Bronze: 5%
/// - Silver: 10%
/// - Gold: 15%
/// - Platinum: 20%
/// 
/// Logic OOP:
/// - Kiểm tra user có null không (khách vãng lai)
/// - Lấy VipLevel từ User entity
/// - Gọi User.GetVipDiscountPercent() để lấy % giảm (Encapsulation)
/// </summary>
public class VipPricingStrategy : IPricingStrategy
{
    public string StrategyName => "VIP Pricing";
    
    public string Description => "Giảm giá theo cấp độ VIP: Bronze 5%, Silver 10%, Gold 15%, Platinum 20%";

    /// <summary>
    /// Tính giá sau khi áp dụng giảm giá VIP
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng - Lấy VipLevel từ đây</param>
    /// <returns>Giá sau khi giảm theo VipLevel</returns>
    public decimal CalculatePrice(Product product, User? user)
    {
        var discountPercent = GetDiscountPercent(product, user);
        return product.Price * (1 - discountPercent);
    }

    /// <summary>
    /// Lấy phần trăm giảm giá theo VipLevel
    /// </summary>
    public decimal GetDiscountPercent(Product product, User? user)
    {
        // Khách vãng lai hoặc chưa đăng nhập: không giảm
        if (user == null)
            return 0m;

        // Lấy % giảm từ User entity (Encapsulation - logic nằm trong Entity)
        return user.GetVipDiscountPercent();
    }

    /// <summary>
    /// Lấy phần trăm giảm giá theo VipLevel cụ thể (helper method)
    /// </summary>
    public static decimal GetDiscountByVipLevel(VipLevel vipLevel)
    {
        return vipLevel switch
        {
            VipLevel.Platinum => 0.20m,  // 20%
            VipLevel.Gold => 0.15m,      // 15%
            VipLevel.Silver => 0.10m,    // 10%
            VipLevel.Bronze => 0.05m,    // 5%
            _ => 0m                       // 0%
        };
    }
}

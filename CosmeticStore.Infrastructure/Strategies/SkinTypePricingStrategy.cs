using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Strategies;

/// <summary>
/// Strategy tính giá theo loại da - Giảm giá khi User phù hợp với Product
/// 
/// Áp dụng tính ĐA HÌNH (Polymorphism):
/// - Cùng interface IPricingStrategy nhưng hành vi khác nhau
/// - SkinTypePricingStrategy giảm giá khi User.SkinType phù hợp với Product.SkinType
/// 
/// Logic nghiệp vụ:
/// - Nếu User đã hoàn thành Skin Quiz VÀ loại da phù hợp với sản phẩm → Giảm 5%
/// - Khuyến khích khách hàng làm Skin Quiz để được giảm giá
/// - Khuyến khích mua sản phẩm phù hợp với da
/// 
/// Sử dụng cho:
/// - Chức năng 9️⃣: AI Skin Quiz
/// - Gợi ý sản phẩm phù hợp với loại da
/// </summary>
public class SkinTypePricingStrategy : IPricingStrategy
{
    /// <summary>
    /// Phần trăm giảm giá khi loại da phù hợp
    /// </summary>
    private const decimal SKIN_TYPE_MATCH_DISCOUNT = 0.05m; // 5%

    public string StrategyName => "Skin Type Pricing";
    
    public string Description => "Giảm 5% khi loại da của bạn phù hợp với sản phẩm";

    /// <summary>
    /// Tính giá sau khi áp dụng giảm giá theo loại da
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng - Kiểm tra SkinType</param>
    /// <returns>Giá sau khi giảm (nếu phù hợp)</returns>
    public decimal CalculatePrice(Product product, User? user)
    {
        var discountPercent = GetDiscountPercent(product, user);
        return product.Price * (1 - discountPercent);
    }

    /// <summary>
    /// Lấy phần trăm giảm giá theo loại da
    /// </summary>
    public decimal GetDiscountPercent(Product product, User? user)
    {
        // Khách vãng lai: không giảm
        if (user == null)
            return 0m;

        // Chưa hoàn thành Skin Quiz: không giảm
        if (!user.HasCompletedSkinQuiz)
            return 0m;

        // Kiểm tra xem loại da có phù hợp không (logic trong User entity - Encapsulation)
        if (user.IsSkinTypeMatch(product.SkinType))
            return SKIN_TYPE_MATCH_DISCOUNT;

        return 0m;
    }

    /// <summary>
    /// Kiểm tra xem User có được giảm giá theo loại da không
    /// </summary>
    public bool IsEligibleForDiscount(Product product, User? user)
    {
        return GetDiscountPercent(product, user) > 0;
    }
}


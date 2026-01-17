using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Decorators;

/// <summary>
/// Decorator giảm giá cho sản phẩm cận hạn sử dụng
/// 
/// DECORATOR PATTERN:
/// - Wrap một IPricingStrategy và thêm logic giảm giá cận hạn
/// - Tự động kiểm tra ExpiryDate của Product
/// 
/// Logic nghiệp vụ:
/// - Còn <= 30 ngày: Giảm 15%
/// - Còn <= 14 ngày: Giảm 25%
/// - Còn <= 7 ngày: Giảm 40%
/// 
/// Phục vụ chức năng 1️⃣1️⃣: Quản lý hạn sử dụng mỹ phẩm
/// </summary>
public class ExpiryDiscountDecorator : PriceDecorator
{
    /// <summary>
    /// Các mức giảm giá theo số ngày còn lại
    /// </summary>
    private const int CRITICAL_DAYS = 7;      // <= 7 ngày
    private const int WARNING_DAYS = 14;      // <= 14 ngày  
    private const int NOTICE_DAYS = 30;       // <= 30 ngày

    private const decimal CRITICAL_DISCOUNT = 0.40m;  // 40%
    private const decimal WARNING_DISCOUNT = 0.25m;   // 25%
    private const decimal NOTICE_DISCOUNT = 0.15m;    // 15%

    public override string StrategyName => "Expiry Discount";
    
    public override string Description => "Giảm giá sản phẩm cận hạn: <=7 ngày -40%, <=14 ngày -25%, <=30 ngày -15%";

    public ExpiryDiscountDecorator(IPricingStrategy innerStrategy) : base(innerStrategy)
    {
    }

    /// <summary>
    /// Tính giá sau khi áp dụng giảm giá cận hạn
    /// Lấy giá từ inner strategy rồi giảm thêm nếu cận hạn
    /// </summary>
    public override decimal CalculatePrice(Product product, User? user)
    {
        // Lấy giá từ inner strategy (có thể đã được giảm bởi VIP, SkinType...)
        var innerPrice = GetInnerPrice(product, user);
        
        // Tính phần trăm giảm thêm do cận hạn
        var expiryDiscount = GetExpiryDiscountPercent(product);
        
        // Áp dụng giảm giá cận hạn
        return innerPrice * (1 - expiryDiscount);
    }

    /// <summary>
    /// Lấy phần trăm giảm giá của decorator này
    /// </summary>
    public override decimal GetDiscountPercent(Product product, User? user)
    {
        return GetExpiryDiscountPercent(product);
    }

    /// <summary>
    /// Tính phần trăm giảm giá dựa trên số ngày còn lại trước khi hết hạn
    /// </summary>
    private decimal GetExpiryDiscountPercent(Product product)
    {
        // Không có ngày hết hạn -> không giảm
        if (!product.ExpiryDate.HasValue)
            return 0m;

        var daysUntilExpiry = product.GetDaysUntilExpiry();
        
        // Đã hết hạn hoặc null -> không giảm (không nên bán)
        if (!daysUntilExpiry.HasValue || daysUntilExpiry <= 0)
            return 0m;

        // Áp dụng mức giảm theo số ngày còn lại
        return daysUntilExpiry.Value switch
        {
            <= CRITICAL_DAYS => CRITICAL_DISCOUNT,  // <= 7 ngày: giảm 40%
            <= WARNING_DAYS => WARNING_DISCOUNT,    // <= 14 ngày: giảm 25%
            <= NOTICE_DAYS => NOTICE_DISCOUNT,      // <= 30 ngày: giảm 15%
            _ => 0m                                  // > 30 ngày: không giảm
        };
    }

    /// <summary>
    /// Kiểm tra sản phẩm có được giảm giá cận hạn không
    /// </summary>
    public bool IsEligibleForExpiryDiscount(Product product)
    {
        return GetExpiryDiscountPercent(product) > 0;
    }

    /// <summary>
    /// Lấy mức cảnh báo hạn sử dụng
    /// </summary>
    public string GetExpiryWarningLevel(Product product)
    {
        if (!product.ExpiryDate.HasValue)
            return "Không có hạn sử dụng";

        var daysUntilExpiry = product.GetDaysUntilExpiry();
        
        if (!daysUntilExpiry.HasValue || daysUntilExpiry <= 0)
            return "Đã hết hạn";

        return daysUntilExpiry.Value switch
        {
            <= CRITICAL_DAYS => $"🔴 Cận hạn nghiêm trọng ({daysUntilExpiry} ngày) - Giảm 40%",
            <= WARNING_DAYS => $"🟠 Cận hạn cảnh báo ({daysUntilExpiry} ngày) - Giảm 25%",
            <= NOTICE_DAYS => $"🟡 Sắp hết hạn ({daysUntilExpiry} ngày) - Giảm 15%",
            _ => $"🟢 Còn hạn ({daysUntilExpiry} ngày)"
        };
    }
}


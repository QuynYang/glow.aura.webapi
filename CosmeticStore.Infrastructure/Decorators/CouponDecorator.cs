using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Decorators;

/// <summary>
/// Decorator giảm giá theo mã giảm giá (Coupon)
/// 
/// DECORATOR PATTERN:
/// - Wrap một IPricingStrategy và thêm logic mã giảm giá
/// - Hỗ trợ giảm theo phần trăm hoặc số tiền cố định
/// 
/// Các loại Coupon:
/// - Percentage: Giảm theo % (ví dụ: SALE10 giảm 10%)
/// - FixedAmount: Giảm số tiền cố định (ví dụ: GIAM50K giảm 50,000 VND)
/// 
/// Ví dụ:
/// Giá sau các giảm giá khác: 80,000 VND
/// Coupon giảm 10%: 80,000 × 0.90 = 72,000 VND
/// Coupon giảm 20,000 VND: 80,000 - 20,000 = 60,000 VND
/// </summary>
public class CouponDecorator : PriceDecorator
{
    private readonly string _couponCode;
    private readonly CouponType _couponType;
    private readonly decimal _discountValue;
    private readonly decimal? _minimumOrderValue;
    private readonly decimal? _maximumDiscount;

    public override string StrategyName => "Coupon Discount";
    
    public override string Description => _couponType == CouponType.Percentage 
        ? $"Mã giảm giá {_couponCode}: -{_discountValue}%"
        : $"Mã giảm giá {_couponCode}: -{_discountValue:N0} VND";

    /// <summary>
    /// Constructor với đầy đủ thông tin coupon
    /// </summary>
    /// <param name="innerStrategy">Strategy bên trong</param>
    /// <param name="couponCode">Mã giảm giá</param>
    /// <param name="couponType">Loại giảm giá (Percentage hoặc FixedAmount)</param>
    /// <param name="discountValue">Giá trị giảm (% hoặc VND)</param>
    /// <param name="minimumOrderValue">Giá trị đơn hàng tối thiểu</param>
    /// <param name="maximumDiscount">Giảm tối đa (cho loại Percentage)</param>
    public CouponDecorator(
        IPricingStrategy innerStrategy,
        string couponCode,
        CouponType couponType,
        decimal discountValue,
        decimal? minimumOrderValue = null,
        decimal? maximumDiscount = null) : base(innerStrategy)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
            throw new ArgumentException("Mã giảm giá không được để trống", nameof(couponCode));
        
        if (discountValue <= 0)
            throw new ArgumentException("Giá trị giảm phải lớn hơn 0", nameof(discountValue));

        if (couponType == CouponType.Percentage && discountValue > 100)
            throw new ArgumentException("Phần trăm giảm không được vượt quá 100%", nameof(discountValue));

        _couponCode = couponCode.ToUpper();
        _couponType = couponType;
        _discountValue = discountValue;
        _minimumOrderValue = minimumOrderValue;
        _maximumDiscount = maximumDiscount;
    }

    /// <summary>
    /// Tính giá sau khi áp dụng mã giảm giá
    /// </summary>
    public override decimal CalculatePrice(Product product, User? user)
    {
        var innerPrice = GetInnerPrice(product, user);
        
        // Kiểm tra giá trị đơn hàng tối thiểu
        if (_minimumOrderValue.HasValue && innerPrice < _minimumOrderValue.Value)
        {
            // Không đủ điều kiện áp dụng coupon
            return innerPrice;
        }

        decimal discount;
        
        if (_couponType == CouponType.Percentage)
        {
            discount = innerPrice * (_discountValue / 100m);
            
            // Áp dụng giới hạn giảm tối đa
            if (_maximumDiscount.HasValue && discount > _maximumDiscount.Value)
            {
                discount = _maximumDiscount.Value;
            }
        }
        else // FixedAmount
        {
            discount = _discountValue;
        }

        // Đảm bảo giá không âm
        var finalPrice = innerPrice - discount;
        return finalPrice > 0 ? finalPrice : 0;
    }

    /// <summary>
    /// Lấy phần trăm giảm giá (ước tính cho loại FixedAmount)
    /// </summary>
    public override decimal GetDiscountPercent(Product product, User? user)
    {
        if (_couponType == CouponType.Percentage)
        {
            return _discountValue / 100m;
        }
        
        // Tính % tương đương cho FixedAmount
        var innerPrice = GetInnerPrice(product, user);
        if (innerPrice <= 0) return 0m;
        
        return Math.Min(_discountValue / innerPrice, 1m);
    }

    /// <summary>
    /// Lấy thông tin mã giảm giá
    /// </summary>
    public string GetCouponCode() => _couponCode;

    /// <summary>
    /// Kiểm tra xem giá có đủ điều kiện áp dụng coupon không
    /// </summary>
    public bool IsEligible(decimal price)
    {
        if (!_minimumOrderValue.HasValue)
            return true;
        
        return price >= _minimumOrderValue.Value;
    }

    /// <summary>
    /// Lấy thông tin coupon để hiển thị
    /// </summary>
    public string GetCouponInfo()
    {
        var info = _couponType == CouponType.Percentage
            ? $"🎫 {_couponCode}: Giảm {_discountValue}%"
            : $"🎫 {_couponCode}: Giảm {_discountValue:N0} VND";

        if (_minimumOrderValue.HasValue)
            info += $" (Đơn tối thiểu {_minimumOrderValue:N0} VND)";
        
        if (_maximumDiscount.HasValue)
            info += $" (Tối đa {_maximumDiscount:N0} VND)";

        return info;
    }
}

/// <summary>
/// Loại mã giảm giá
/// </summary>
public enum CouponType
{
    /// <summary>
    /// Giảm theo phần trăm (ví dụ: 10%)
    /// </summary>
    Percentage = 0,

    /// <summary>
    /// Giảm số tiền cố định (ví dụ: 50,000 VND)
    /// </summary>
    FixedAmount = 1
}


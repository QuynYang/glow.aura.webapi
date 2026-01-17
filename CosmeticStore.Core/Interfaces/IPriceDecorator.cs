using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Abstract class Decorator Pattern cho việc cộng dồn khuyến mãi
/// 
/// DECORATOR PATTERN:
/// - Thêm chức năng mới cho object mà không thay đổi cấu trúc
/// - Sử dụng Composition: Chứa một IPricingStrategy bên trong
/// - Cho phép "wrap" nhiều lớp decorator lên nhau
/// 
/// Ví dụ cộng dồn:
/// Giá gốc: 100,000 VND
/// → VipPricingStrategy: 100,000 × 0.90 = 90,000 (giảm 10% VIP)
/// → ExpiryDiscountDecorator: 90,000 × 0.85 = 76,500 (giảm thêm 15% cận hạn)
/// → FlashSaleDecorator: 76,500 × 0.80 = 61,200 (giảm thêm 20% Flash Sale)
/// → Giá cuối: 61,200 VND
/// 
/// Nguyên tắc OOP:
/// - Composition over Inheritance
/// - Open/Closed Principle: Thêm decorator mới mà không sửa code cũ
/// </summary>
public abstract class PriceDecorator : IPricingStrategy
{
    /// <summary>
    /// Strategy hoặc Decorator bên trong (Composition)
    /// Đây là "wrapped" component
    /// </summary>
    protected readonly IPricingStrategy _innerStrategy;

    /// <summary>
    /// Constructor - Nhận strategy cần wrap
    /// </summary>
    /// <param name="innerStrategy">Strategy hoặc Decorator cần bọc</param>
    protected PriceDecorator(IPricingStrategy innerStrategy)
    {
        _innerStrategy = innerStrategy ?? throw new ArgumentNullException(nameof(innerStrategy));
    }

    /// <summary>
    /// Tên decorator
    /// </summary>
    public abstract string StrategyName { get; }

    /// <summary>
    /// Mô tả decorator
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Tính giá sau khi áp dụng decorator
    /// Lấy giá từ inner strategy rồi áp dụng giảm giá thêm
    /// </summary>
    public abstract decimal CalculatePrice(Product product, User? user);

    /// <summary>
    /// Lấy phần trăm giảm giá của decorator này
    /// </summary>
    public abstract decimal GetDiscountPercent(Product product, User? user);

    /// <summary>
    /// Lấy giá từ inner strategy (để decorator con sử dụng)
    /// </summary>
    protected decimal GetInnerPrice(Product product, User? user)
    {
        return _innerStrategy.CalculatePrice(product, user);
    }

    /// <summary>
    /// Lấy tổng % giảm từ inner strategy
    /// </summary>
    protected decimal GetInnerDiscountPercent(Product product, User? user)
    {
        return _innerStrategy.GetDiscountPercent(product, user);
    }
}


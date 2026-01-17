using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Decorators;

/// <summary>
/// Decorator giảm giá Flash Sale
/// 
/// DECORATOR PATTERN:
/// - Wrap một IPricingStrategy và thêm logic Flash Sale
/// - Kiểm tra cờ IsFlashSale và FlashSaleEndTime của Product
/// 
/// Logic nghiệp vụ:
/// - Nếu Product.IsFlashSale = true VÀ FlashSaleEndTime > Now
/// - Áp dụng giảm giá theo Product.FlashSaleDiscount
/// 
/// Ví dụ:
/// Giá sau VIP: 90,000 VND
/// Flash Sale giảm 20%: 90,000 × 0.80 = 72,000 VND
/// </summary>
public class FlashSaleDecorator : PriceDecorator
{
    public override string StrategyName => "Flash Sale";
    
    public override string Description => "Giảm giá Flash Sale theo % được cấu hình trên sản phẩm";

    public FlashSaleDecorator(IPricingStrategy innerStrategy) : base(innerStrategy)
    {
    }

    /// <summary>
    /// Tính giá sau khi áp dụng Flash Sale
    /// </summary>
    public override decimal CalculatePrice(Product product, User? user)
    {
        // Lấy giá từ inner strategy
        var innerPrice = GetInnerPrice(product, user);
        
        // Kiểm tra và áp dụng Flash Sale
        if (product.IsInActiveFlashSale())
        {
            var flashSaleDiscount = product.FlashSaleDiscount / 100m;
            return innerPrice * (1 - flashSaleDiscount);
        }

        return innerPrice;
    }

    /// <summary>
    /// Lấy phần trăm giảm giá Flash Sale
    /// </summary>
    public override decimal GetDiscountPercent(Product product, User? user)
    {
        if (product.IsInActiveFlashSale())
        {
            return product.FlashSaleDiscount / 100m;
        }
        return 0m;
    }

    /// <summary>
    /// Kiểm tra sản phẩm có đang Flash Sale không
    /// </summary>
    public bool IsFlashSaleActive(Product product)
    {
        return product.IsInActiveFlashSale();
    }

    /// <summary>
    /// Lấy thời gian còn lại của Flash Sale
    /// </summary>
    public TimeSpan? GetTimeRemaining(Product product)
    {
        if (!product.IsInActiveFlashSale())
            return null;

        return product.FlashSaleEndTime!.Value - DateTime.UtcNow;
    }

    /// <summary>
    /// Lấy thông tin Flash Sale để hiển thị
    /// </summary>
    public string GetFlashSaleInfo(Product product)
    {
        if (!product.IsInActiveFlashSale())
            return "Không có Flash Sale";

        var timeRemaining = GetTimeRemaining(product);
        if (!timeRemaining.HasValue)
            return "Flash Sale đã kết thúc";

        var hours = (int)timeRemaining.Value.TotalHours;
        var minutes = timeRemaining.Value.Minutes;

        return $"⚡ Flash Sale -{product.FlashSaleDiscount}% | Còn {hours}h {minutes}m";
    }
}


using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho Pricing Service - Orchestrator của Strategy + Decorator Pattern
/// 
/// MỤC ĐÍCH:
/// - Quyết định Strategy nào sẽ được sử dụng cho User
/// - Quyết định bao nhiêu Decorator sẽ được wrap
/// - Tính toán giá cuối cùng sau tất cả các giảm giá
/// 
/// VÍ DỤ LUỒNG XỬ LÝ:
/// 1. User VIP Gold, sản phẩm cận hạn 10 ngày, đang Flash Sale
/// 2. Base Strategy: VipPricingStrategy (giảm 15%)
/// 3. Wrap ExpiryDiscountDecorator (giảm thêm 25%)
/// 4. Wrap FlashSaleDecorator (giảm thêm 20%)
/// 5. Giá cuối = 100,000 × 0.85 × 0.75 × 0.80 = 51,000 VND
/// </summary>
public interface IPricingService
{
    /// <summary>
    /// Tính giá cuối cùng sau khi áp dụng tất cả chiến lược và giảm giá
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng (null = khách vãng lai)</param>
    /// <param name="couponCode">Mã giảm giá (optional)</param>
    /// <returns>Kết quả tính giá chi tiết</returns>
    PricingResult CalculateFinalPrice(Product product, User? user, string? couponCode = null);

    /// <summary>
    /// Tính giá cho nhiều sản phẩm (giỏ hàng)
    /// </summary>
    /// <param name="products">Danh sách sản phẩm</param>
    /// <param name="user">Người dùng</param>
    /// <param name="couponCode">Mã giảm giá</param>
    /// <returns>Kết quả tính giá cho từng sản phẩm</returns>
    IEnumerable<PricingResult> CalculateCartPrices(IEnumerable<Product> products, User? user, string? couponCode = null);

    /// <summary>
    /// Xây dựng chuỗi Strategy + Decorator phù hợp
    /// </summary>
    /// <param name="product">Sản phẩm</param>
    /// <param name="user">Người dùng</param>
    /// <param name="couponCode">Mã giảm giá</param>
    /// <returns>Strategy đã được wrap bởi các Decorator</returns>
    IPricingStrategy BuildPricingChain(Product product, User? user, string? couponCode = null);
}

/// <summary>
/// Kết quả tính giá chi tiết - Hiển thị từng bước giảm giá
/// </summary>
public class PricingResult
{
    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Tên sản phẩm
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Giá gốc
    /// </summary>
    public decimal OriginalPrice { get; set; }

    /// <summary>
    /// Giá cuối cùng sau tất cả giảm giá
    /// </summary>
    public decimal FinalPrice { get; set; }

    /// <summary>
    /// Tổng số tiền được giảm
    /// </summary>
    public decimal TotalDiscount => OriginalPrice - FinalPrice;

    /// <summary>
    /// Tổng phần trăm giảm
    /// </summary>
    public decimal TotalDiscountPercent => OriginalPrice > 0 
        ? Math.Round((TotalDiscount / OriginalPrice) * 100, 2) 
        : 0;

    /// <summary>
    /// Chi tiết các bước giảm giá đã áp dụng
    /// </summary>
    public List<DiscountDetail> AppliedDiscounts { get; set; } = new();

    /// <summary>
    /// Các cảnh báo (ví dụ: sản phẩm cận hạn)
    /// </summary>
    public List<string> Warnings { get; set; } = new();
}

/// <summary>
/// Chi tiết một bước giảm giá
/// </summary>
public class DiscountDetail
{
    /// <summary>
    /// Tên loại giảm giá
    /// </summary>
    public string DiscountType { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Phần trăm giảm
    /// </summary>
    public decimal DiscountPercent { get; set; }

    /// <summary>
    /// Số tiền giảm
    /// </summary>
    public decimal DiscountAmount { get; set; }

    /// <summary>
    /// Giá sau khi áp dụng giảm giá này
    /// </summary>
    public decimal PriceAfterDiscount { get; set; }
}


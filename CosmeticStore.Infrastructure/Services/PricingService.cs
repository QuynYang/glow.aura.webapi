using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.Decorators;
using CosmeticStore.Infrastructure.Strategies;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Pricing Service - Orchestrator của Strategy + Decorator Pattern
/// 
/// NHIỆM VỤ:
/// 1. Chọn Strategy phù hợp dựa trên User (VIP, SkinType, Standard)
/// 2. Wrap các Decorator cần thiết (Expiry, FlashSale, Coupon)
/// 3. Tính toán và trả về kết quả chi tiết
/// 
/// LUỒNG XỬ LÝ:
/// User VIP Gold + Sản phẩm cận hạn 10 ngày + Flash Sale 20% + Coupon 10%
/// 
/// Step 1: Base Strategy = VipPricingStrategy
/// Step 2: Wrap ExpiryDiscountDecorator (25% cho <=14 ngày)
/// Step 3: Wrap FlashSaleDecorator (20%)
/// Step 4: Wrap CouponDecorator (10%)
/// 
/// Giá gốc: 100,000 VND
/// → Sau VIP 15%: 85,000 VND
/// → Sau Expiry 25%: 63,750 VND
/// → Sau Flash Sale 20%: 51,000 VND
/// → Sau Coupon 10%: 45,900 VND
/// </summary>
public class PricingService : IPricingService
{
    // Danh sách các coupon hợp lệ (trong thực tế sẽ lấy từ database)
    private static readonly Dictionary<string, CouponInfo> _validCoupons = new()
    {
        { "WELCOME10", new CouponInfo(CouponType.Percentage, 10, null, null) },
        { "SALE20", new CouponInfo(CouponType.Percentage, 20, 500000, 100000) },
        { "GIAM50K", new CouponInfo(CouponType.FixedAmount, 50000, 200000, null) },
        { "VIP30", new CouponInfo(CouponType.Percentage, 30, 1000000, 200000) },
        { "FREESHIP", new CouponInfo(CouponType.FixedAmount, 30000, null, null) }
    };

    /// <summary>
    /// Tính giá cuối cùng với đầy đủ chi tiết
    /// </summary>
    public PricingResult CalculateFinalPrice(Product product, User? user, string? couponCode = null)
    {
        var result = new PricingResult
        {
            ProductId = product.Id,
            ProductName = product.Name,
            OriginalPrice = product.Price
        };

        decimal currentPrice = product.Price;

        // Step 1: Áp dụng Base Strategy (VIP hoặc SkinType hoặc Standard)
        currentPrice = ApplyBaseStrategy(product, user, result, currentPrice);

        // Step 2: Áp dụng Expiry Discount nếu có
        currentPrice = ApplyExpiryDiscount(product, result, currentPrice);

        // Step 3: Áp dụng Flash Sale nếu có
        currentPrice = ApplyFlashSale(product, result, currentPrice);

        // Step 4: Áp dụng Coupon nếu có
        currentPrice = ApplyCoupon(couponCode, result, currentPrice);

        // Thêm cảnh báo nếu cần
        AddWarnings(product, result);

        result.FinalPrice = Math.Round(currentPrice, 0); // Làm tròn đến VND
        return result;
    }

    /// <summary>
    /// Tính giá cho nhiều sản phẩm (giỏ hàng)
    /// </summary>
    public IEnumerable<PricingResult> CalculateCartPrices(IEnumerable<Product> products, User? user, string? couponCode = null)
    {
        return products.Select(p => CalculateFinalPrice(p, user, couponCode));
    }

    /// <summary>
    /// Xây dựng chuỗi Strategy + Decorator
    /// </summary>
    public IPricingStrategy BuildPricingChain(Product product, User? user, string? couponCode = null)
    {
        // Step 1: Chọn Base Strategy
        IPricingStrategy strategy = SelectBaseStrategy(user, product);

        // Step 2: Wrap ExpiryDiscountDecorator nếu sản phẩm cận hạn
        if (product.ExpiryDate.HasValue && product.IsExpiringSoon(30))
        {
            strategy = new ExpiryDiscountDecorator(strategy);
        }

        // Step 3: Wrap FlashSaleDecorator nếu đang Flash Sale
        if (product.IsInActiveFlashSale())
        {
            strategy = new FlashSaleDecorator(strategy);
        }

        // Step 4: Wrap CouponDecorator nếu có mã hợp lệ
        if (!string.IsNullOrWhiteSpace(couponCode) && TryGetCoupon(couponCode, out var couponInfo))
        {
            strategy = new CouponDecorator(
                strategy,
                couponCode,
                couponInfo.Type,
                couponInfo.Value,
                couponInfo.MinOrderValue,
                couponInfo.MaxDiscount
            );
        }

        return strategy;
    }

    #region Private Methods

    /// <summary>
    /// Chọn Base Strategy dựa trên User
    /// </summary>
    private IPricingStrategy SelectBaseStrategy(User? user, Product product)
    {
        // Khách vãng lai -> Standard
        if (user == null)
            return new StandardPricingStrategy();

        // Ưu tiên VIP nếu có level cao
        if (user.VipLevel != VipLevel.None)
            return new VipPricingStrategy();

        // Kiểm tra SkinType match
        if (user.HasCompletedSkinQuiz && user.IsSkinTypeMatch(product.SkinType))
            return new SkinTypePricingStrategy();

        // Mặc định
        return new StandardPricingStrategy();
    }

    /// <summary>
    /// Áp dụng Base Strategy và ghi nhận
    /// </summary>
    private decimal ApplyBaseStrategy(Product product, User? user, PricingResult result, decimal currentPrice)
    {
        var strategy = SelectBaseStrategy(user, product);
        var newPrice = strategy.CalculatePrice(product, user);
        var discountPercent = strategy.GetDiscountPercent(product, user);

        if (discountPercent > 0)
        {
            result.AppliedDiscounts.Add(new DiscountDetail
            {
                DiscountType = strategy.StrategyName,
                Description = strategy.Description,
                DiscountPercent = discountPercent * 100,
                DiscountAmount = currentPrice - newPrice,
                PriceAfterDiscount = newPrice
            });
        }

        return newPrice;
    }

    /// <summary>
    /// Áp dụng Expiry Discount
    /// </summary>
    private decimal ApplyExpiryDiscount(Product product, PricingResult result, decimal currentPrice)
    {
        if (!product.ExpiryDate.HasValue || !product.IsExpiringSoon(30))
            return currentPrice;

        var decorator = new ExpiryDiscountDecorator(new StandardPricingStrategy());
        var discountPercent = decorator.GetDiscountPercent(product, null);

        if (discountPercent > 0)
        {
            var newPrice = currentPrice * (1 - discountPercent);
            result.AppliedDiscounts.Add(new DiscountDetail
            {
                DiscountType = decorator.StrategyName,
                Description = decorator.GetExpiryWarningLevel(product),
                DiscountPercent = discountPercent * 100,
                DiscountAmount = currentPrice - newPrice,
                PriceAfterDiscount = newPrice
            });
            return newPrice;
        }

        return currentPrice;
    }

    /// <summary>
    /// Áp dụng Flash Sale
    /// </summary>
    private decimal ApplyFlashSale(Product product, PricingResult result, decimal currentPrice)
    {
        if (!product.IsInActiveFlashSale())
            return currentPrice;

        var discountPercent = product.FlashSaleDiscount / 100m;
        var newPrice = currentPrice * (1 - discountPercent);

        result.AppliedDiscounts.Add(new DiscountDetail
        {
            DiscountType = "Flash Sale",
            Description = $"⚡ Flash Sale giảm {product.FlashSaleDiscount}%",
            DiscountPercent = product.FlashSaleDiscount,
            DiscountAmount = currentPrice - newPrice,
            PriceAfterDiscount = newPrice
        });

        return newPrice;
    }

    /// <summary>
    /// Áp dụng Coupon
    /// </summary>
    private decimal ApplyCoupon(string? couponCode, PricingResult result, decimal currentPrice)
    {
        if (string.IsNullOrWhiteSpace(couponCode))
            return currentPrice;

        if (!TryGetCoupon(couponCode, out var couponInfo))
        {
            result.Warnings.Add($"Mã giảm giá '{couponCode}' không hợp lệ");
            return currentPrice;
        }

        // Kiểm tra giá trị đơn hàng tối thiểu
        if (couponInfo.MinOrderValue.HasValue && currentPrice < couponInfo.MinOrderValue.Value)
        {
            result.Warnings.Add($"Đơn hàng chưa đạt giá trị tối thiểu {couponInfo.MinOrderValue:N0} VND để áp dụng mã {couponCode}");
            return currentPrice;
        }

        decimal discount;
        decimal discountPercent;

        if (couponInfo.Type == CouponType.Percentage)
        {
            discountPercent = couponInfo.Value;
            discount = currentPrice * (couponInfo.Value / 100m);

            if (couponInfo.MaxDiscount.HasValue && discount > couponInfo.MaxDiscount.Value)
            {
                discount = couponInfo.MaxDiscount.Value;
            }
        }
        else
        {
            discount = couponInfo.Value;
            discountPercent = currentPrice > 0 ? (discount / currentPrice) * 100 : 0;
        }

        var newPrice = Math.Max(0, currentPrice - discount);

        result.AppliedDiscounts.Add(new DiscountDetail
        {
            DiscountType = "Coupon",
            Description = $"🎫 Mã giảm giá: {couponCode.ToUpper()}",
            DiscountPercent = discountPercent,
            DiscountAmount = discount,
            PriceAfterDiscount = newPrice
        });

        return newPrice;
    }

    /// <summary>
    /// Thêm cảnh báo nếu cần
    /// </summary>
    private void AddWarnings(Product product, PricingResult result)
    {
        // Cảnh báo hết hàng
        if (product.Stock == 0)
        {
            result.Warnings.Add("⚠️ Sản phẩm đã hết hàng");
        }
        else if (product.Stock <= 5)
        {
            result.Warnings.Add($"⚠️ Chỉ còn {product.Stock} sản phẩm");
        }

        // Cảnh báo hết hạn
        if (product.IsExpired())
        {
            result.Warnings.Add("🚫 Sản phẩm đã hết hạn sử dụng");
        }
        else if (product.IsExpiringSoon(7))
        {
            result.Warnings.Add($"⚠️ Sản phẩm sắp hết hạn trong {product.GetDaysUntilExpiry()} ngày");
        }
    }

    /// <summary>
    /// Lấy thông tin coupon
    /// </summary>
    private bool TryGetCoupon(string couponCode, out CouponInfo couponInfo)
    {
        return _validCoupons.TryGetValue(couponCode.ToUpper(), out couponInfo!);
    }

    #endregion
}

/// <summary>
/// Thông tin Coupon (sẽ lưu trong DB thực tế)
/// </summary>
internal record CouponInfo(CouponType Type, decimal Value, decimal? MinOrderValue, decimal? MaxDiscount);


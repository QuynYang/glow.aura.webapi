using CosmeticStore.Core.Builders;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using BuilderDiscountDetail = CosmeticStore.Core.Builders.DiscountDetail;

namespace CosmeticStore.Infrastructure.Builders;

/// <summary>
/// Order Builder - BUILDER PATTERN
/// 
/// Xây dựng đối tượng Order phức tạp từng bước:
/// 1. WithUser() - Set thông tin khách hàng
/// 2. WithItems() - Thêm sản phẩm (tính giá qua Strategy Pattern)
/// 3. WithShippingAddress() - Set địa chỉ giao hàng
/// 4. WithPaymentMethod() - Chọn phương thức thanh toán
/// 5. WithVoucher() - Áp dụng mã giảm giá (Decorator Pattern)
/// 6. Build() - Validate và trả về Order hoàn chỉnh
/// 
/// Kết hợp các Pattern:
/// - BUILDER: Xây dựng từng bước
/// - STRATEGY: Tính giá theo VIP level, SkinType
/// - DECORATOR: Cộng dồn giảm giá (Expiry, FlashSale, Coupon)
/// - ENCAPSULATION: Order có private setters, chỉ build qua Builder
/// </summary>
public class OrderBuilder : IOrderBuilder
{
    private readonly IProductRepository _productRepository;
    private readonly IPricingService _pricingService;
    private readonly ISystemLogger _logger;

    // Internal state
    private User? _user;
    private int? _userId;
    private readonly List<CartItem> _cartItems = new();
    private readonly List<OrderItem> _orderItems = new();
    private string? _shippingAddress;
    private string? _shippingPhone;
    private string? _receiverName;
    private PaymentMethod? _paymentMethod;
    private string? _voucherCode;
    private string? _notes;
    private string? _giftMessage;
    private decimal _giftWrapFee;
    private decimal? _customShippingFee;
    private bool _isExpressDelivery;
    
    // Tracking
    private decimal _originalTotal;
    private decimal _totalDiscount;
    private readonly List<BuilderDiscountDetail> _appliedDiscounts = new();
    private readonly List<string> _warnings = new();
    private readonly List<string> _validationErrors = new();

    public OrderBuilder(
        IProductRepository productRepository,
        IPricingService pricingService,
        ISystemLogger logger)
    {
        _productRepository = productRepository;
        _pricingService = pricingService;
        _logger = logger;
    }

    #region Required Steps

    /// <summary>
    /// Set User cho đơn hàng
    /// </summary>
    public IOrderBuilder WithUser(User user)
    {
        _user = user ?? throw new ArgumentNullException(nameof(user));
        _userId = user.Id;
        
        _logger.LogDebug($"[OrderBuilder] WithUser: {user.FullName} (VIP: {user.VipLevel})");
        return this;
    }

    /// <summary>
    /// Set User ID (khi không có User object)
    /// </summary>
    public IOrderBuilder WithUserId(int userId)
    {
        if (userId <= 0)
            throw new ArgumentException("User ID phải lớn hơn 0", nameof(userId));
            
        _userId = userId;
        _logger.LogDebug($"[OrderBuilder] WithUserId: {userId}");
        return this;
    }

    /// <summary>
    /// Thêm sản phẩm từ giỏ hàng
    /// Tính giá qua Strategy + Decorator Pattern
    /// </summary>
    public IOrderBuilder WithItems(IEnumerable<CartItem> cartItems)
    {
        _cartItems.Clear();
        _orderItems.Clear();
        _appliedDiscounts.Clear();
        _originalTotal = 0;
        _totalDiscount = 0;

        foreach (var item in cartItems)
        {
            if (item.Quantity <= 0)
            {
                _warnings.Add($"Sản phẩm ID {item.ProductId} có số lượng không hợp lệ, đã bỏ qua");
                continue;
            }
            _cartItems.Add(item);
        }

        _logger.LogDebug($"[OrderBuilder] WithItems: {_cartItems.Count} items");
        return this;
    }

    /// <summary>
    /// Set địa chỉ giao hàng
    /// </summary>
    public IOrderBuilder WithShippingAddress(string address, string phone, string receiverName)
    {
        _shippingAddress = address?.Trim();
        _shippingPhone = phone?.Trim();
        _receiverName = receiverName?.Trim();
        
        _logger.LogDebug($"[OrderBuilder] WithShippingAddress: {receiverName}, {address}");
        return this;
    }

    /// <summary>
    /// Set phương thức thanh toán
    /// </summary>
    public IOrderBuilder WithPaymentMethod(PaymentMethod method)
    {
        _paymentMethod = method;
        _logger.LogDebug($"[OrderBuilder] WithPaymentMethod: {method}");
        return this;
    }

    #endregion

    #region Optional Steps

    /// <summary>
    /// Áp dụng voucher/coupon
    /// </summary>
    public IOrderBuilder WithVoucher(string? voucherCode)
    {
        _voucherCode = voucherCode?.Trim()?.ToUpperInvariant();
        
        if (!string.IsNullOrEmpty(_voucherCode))
        {
            _logger.LogDebug($"[OrderBuilder] WithVoucher: {_voucherCode}");
        }
        return this;
    }

    /// <summary>
    /// Thêm ghi chú
    /// </summary>
    public IOrderBuilder WithNotes(string? notes)
    {
        _notes = notes?.Trim();
        return this;
    }

    /// <summary>
    /// Yêu cầu gói quà
    /// </summary>
    public IOrderBuilder WithGiftWrap(string? giftMessage, decimal giftWrapFee = 0)
    {
        _giftMessage = giftMessage?.Trim();
        _giftWrapFee = giftWrapFee >= 0 ? giftWrapFee : 0;
        
        if (!string.IsNullOrEmpty(_giftMessage))
        {
            _logger.LogDebug($"[OrderBuilder] WithGiftWrap: {giftWrapFee:N0} VND");
        }
        return this;
    }

    /// <summary>
    /// Set phí ship custom
    /// </summary>
    public IOrderBuilder WithShippingFee(decimal shippingFee)
    {
        _customShippingFee = shippingFee >= 0 ? shippingFee : 0;
        return this;
    }

    /// <summary>
    /// Giao hàng nhanh
    /// </summary>
    public IOrderBuilder WithExpressDelivery(bool isExpress = true)
    {
        _isExpressDelivery = isExpress;
        
        if (isExpress)
        {
            _logger.LogDebug("[OrderBuilder] WithExpressDelivery: Enabled");
        }
        return this;
    }

    #endregion

    #region Build

    /// <summary>
    /// Build và trả về Order hoàn chỉnh
    /// </summary>
    public Order Build()
    {
        _validationErrors.Clear();
        
        // 1. Validate
        if (!ValidateRequiredFields())
        {
            var errorMessage = string.Join("; ", _validationErrors);
            throw new InvalidOperationException($"Không thể tạo đơn hàng: {errorMessage}");
        }

        // 2. Load products và tính giá
        ProcessCartItems();

        // 3. Validate sau khi xử lý
        if (!_orderItems.Any())
        {
            throw new InvalidOperationException("Không có sản phẩm hợp lệ trong đơn hàng");
        }

        // 4. Tạo Order (sử dụng constructor với các giá trị required)
        var order = new Order(
            userId: _userId!.Value,
            shippingAddress: _shippingAddress!,
            shippingPhone: _shippingPhone!,
            receiverName: _receiverName!,
            paymentMethod: _paymentMethod!.Value,
            notes: BuildNotes(),
            couponCode: _voucherCode
        );

        // 5. Thêm các OrderItem
        foreach (var item in _orderItems)
        {
            order.AddItem(item);
        }

        // 6. Set phí ship
        var shippingFee = CalculateShippingFee();
        order.SetShippingFee(shippingFee);

        // 7. Áp dụng discount tổng (nếu có)
        if (_totalDiscount > 0)
        {
            order.ApplyDiscount(_totalDiscount);
        }

        _logger.LogInfo("[OrderBuilder] Order built successfully", new
        {
            OrderNumber = order.OrderNumber,
            ItemCount = _orderItems.Count,
            OriginalTotal = _originalTotal,
            TotalDiscount = _totalDiscount,
            ShippingFee = shippingFee,
            FinalTotal = order.TotalAmount,
            HasVoucher = !string.IsNullOrEmpty(_voucherCode),
            IsExpress = _isExpressDelivery,
            HasGiftWrap = !string.IsNullOrEmpty(_giftMessage)
        });

        return order;
    }

    /// <summary>
    /// Kiểm tra có thể build không
    /// </summary>
    public bool CanBuild()
    {
        _validationErrors.Clear();
        return ValidateRequiredFields() && _cartItems.Any();
    }

    /// <summary>
    /// Lấy danh sách lỗi validation
    /// </summary>
    public IReadOnlyList<string> GetValidationErrors()
    {
        ValidateRequiredFields();
        return _validationErrors.AsReadOnly();
    }

    /// <summary>
    /// Reset builder
    /// </summary>
    public IOrderBuilder Reset()
    {
        _user = null;
        _userId = null;
        _cartItems.Clear();
        _orderItems.Clear();
        _shippingAddress = null;
        _shippingPhone = null;
        _receiverName = null;
        _paymentMethod = null;
        _voucherCode = null;
        _notes = null;
        _giftMessage = null;
        _giftWrapFee = 0;
        _customShippingFee = null;
        _isExpressDelivery = false;
        _originalTotal = 0;
        _totalDiscount = 0;
        _appliedDiscounts.Clear();
        _warnings.Clear();
        _validationErrors.Clear();

        _logger.LogDebug("[OrderBuilder] Reset");
        return this;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Validate các trường bắt buộc
    /// </summary>
    private bool ValidateRequiredFields()
    {
        if (_userId == null)
            _validationErrors.Add("Chưa có thông tin khách hàng");

        if (!_cartItems.Any())
            _validationErrors.Add("Giỏ hàng trống");

        if (string.IsNullOrWhiteSpace(_shippingAddress))
            _validationErrors.Add("Chưa có địa chỉ giao hàng");

        if (string.IsNullOrWhiteSpace(_shippingPhone))
            _validationErrors.Add("Chưa có số điện thoại");

        if (string.IsNullOrWhiteSpace(_receiverName))
            _validationErrors.Add("Chưa có tên người nhận");

        if (_paymentMethod == null)
            _validationErrors.Add("Chưa chọn phương thức thanh toán");

        return !_validationErrors.Any();
    }

    /// <summary>
    /// Xử lý giỏ hàng: Load product, tính giá
    /// </summary>
    private void ProcessCartItems()
    {
        foreach (var cartItem in _cartItems)
        {
            // Load product nếu chưa có
            var product = cartItem.Product;
            if (product == null)
            {
                product = _productRepository.GetByIdAsync(cartItem.ProductId).Result;
            }

            if (product == null)
            {
                _warnings.Add($"Sản phẩm ID {cartItem.ProductId} không tồn tại");
                continue;
            }

            if (product.IsDeleted)
            {
                _warnings.Add($"Sản phẩm '{product.Name}' đã ngừng kinh doanh");
                continue;
            }

            if (product.Stock < cartItem.Quantity)
            {
                _warnings.Add($"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm");
                continue;
            }

            // Tính giá qua PricingService (Strategy + Decorator Pattern)
            var pricingResult = _pricingService.CalculateFinalPrice(product, _user, _voucherCode);

            // Tạo OrderItem
            var orderItem = new OrderItem(
                productId: product.Id,
                productName: product.Name,
                unitPrice: product.Price,
                discountedPrice: pricingResult.FinalPrice,
                quantity: cartItem.Quantity,
                discountDescription: BuildDiscountDescription(pricingResult)
            );

            _orderItems.Add(orderItem);

            // Tracking
            var itemOriginal = product.Price * cartItem.Quantity;
            var itemFinal = pricingResult.FinalPrice * cartItem.Quantity;
            _originalTotal += itemOriginal;
            
            // Track discounts
            foreach (var discount in pricingResult.AppliedDiscounts)
            {
                _appliedDiscounts.Add(new BuilderDiscountDetail
                {
                    Type = discount.DiscountType,
                    Description = $"{product.Name}: {discount.Description}",
                    Amount = discount.DiscountAmount * cartItem.Quantity,
                    Percent = discount.DiscountPercent
                });
            }

            // Track warnings
            if (pricingResult.Warnings.Any())
            {
                _warnings.AddRange(pricingResult.Warnings.Select(w => $"{product.Name}: {w}"));
            }
        }

        // Tính tổng discount
        _totalDiscount = _appliedDiscounts.Sum(d => d.Amount);
    }

    /// <summary>
    /// Tính phí ship
    /// </summary>
    private decimal CalculateShippingFee()
    {
        // Nếu có custom shipping fee
        if (_customShippingFee.HasValue)
            return _customShippingFee.Value;

        // Tính phí ship mặc định
        var subTotal = _orderItems.Sum(i => i.TotalPrice);
        decimal baseFee = 30000; // 30k cơ bản

        // Miễn phí ship cho đơn >= 500k
        if (subTotal >= 500000)
            baseFee = 0;

        // Phí express
        if (_isExpressDelivery)
            baseFee += 20000;

        // Phí gói quà
        if (!string.IsNullOrEmpty(_giftMessage))
            baseFee += _giftWrapFee;

        // Miễn phí cho VIP Gold+
        if (_user?.VipLevel >= VipLevel.Gold && baseFee > 0)
        {
            _appliedDiscounts.Add(new BuilderDiscountDetail
            {
                Type = "VIP Shipping",
                Description = "Miễn phí ship VIP",
                Amount = baseFee
            });
            baseFee = 0;
        }

        return baseFee;
    }

    /// <summary>
    /// Build mô tả giảm giá từ PricingResult
    /// </summary>
    private static string? BuildDiscountDescription(PricingResult result)
    {
        if (!result.AppliedDiscounts.Any())
            return null;

        var descriptions = result.AppliedDiscounts
            .Select(d => $"{d.DiscountType}: -{d.DiscountPercent}%")
            .ToList();

        return string.Join(", ", descriptions);
    }

    /// <summary>
    /// Build notes bao gồm gift message
    /// </summary>
    private string? BuildNotes()
    {
        var parts = new List<string>();
        
        if (!string.IsNullOrEmpty(_notes))
            parts.Add(_notes);

        if (!string.IsNullOrEmpty(_giftMessage))
            parts.Add($"[Gói quà] {_giftMessage}");

        if (_isExpressDelivery)
            parts.Add("[Giao hàng nhanh]");

        return parts.Any() ? string.Join(" | ", parts) : null;
    }

    #endregion

    #region Extended Methods

    /// <summary>
    /// Lấy kết quả build chi tiết
    /// </summary>
    public OrderBuildResult BuildWithDetails()
    {
        var order = Build();

        return new OrderBuildResult
        {
            Order = order,
            OriginalTotal = _originalTotal,
            TotalDiscount = _totalDiscount,
            AppliedDiscounts = _appliedDiscounts.ToList(),
            Warnings = _warnings.ToList(),
            AppliedVoucherCode = _voucherCode,
            HasGiftWrap = !string.IsNullOrEmpty(_giftMessage),
            IsExpressDelivery = _isExpressDelivery
        };
    }

    /// <summary>
    /// Preview đơn hàng trước khi build (không validate stock)
    /// </summary>
    public OrderBuildResult Preview()
    {
        // Clone cart items để không ảnh hưởng state
        ProcessCartItems();

        return new OrderBuildResult
        {
            Order = null!, // Chưa build
            OriginalTotal = _originalTotal,
            TotalDiscount = _totalDiscount,
            AppliedDiscounts = _appliedDiscounts.ToList(),
            Warnings = _warnings.ToList(),
            AppliedVoucherCode = _voucherCode,
            HasGiftWrap = !string.IsNullOrEmpty(_giftMessage),
            IsExpressDelivery = _isExpressDelivery
        };
    }

    #endregion
}


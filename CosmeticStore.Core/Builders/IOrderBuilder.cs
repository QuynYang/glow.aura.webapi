using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Builders;

/// <summary>
/// Interface cho Order Builder - BUILDER PATTERN
/// 
/// BUILDER PATTERN:
/// - Tách biệt việc xây dựng đối tượng phức tạp khỏi biểu diễn của nó
/// - Cho phép xây dựng từng bước (step by step)
/// - Fluent Interface: Mỗi method trả về chính builder để nối chuỗi
/// 
/// Vấn đề giải quyết:
/// - Order có nhiều thuộc tính: User, Items, Address, Payment, Voucher, Notes...
/// - Constructor dài và dễ sai sót: new Order(a, b, c, d, e, f, g...)
/// - Khó bảo trì khi thêm thuộc tính mới
/// 
/// Giải pháp:
/// var order = builder
///     .WithUser(user)
///     .WithItems(cartItems)
///     .WithShippingAddress(address, phone, name)
///     .WithPaymentMethod(PaymentMethod.Momo)
///     .WithVoucher("SALE20")
///     .WithGiftWrap("Gói quà sinh nhật")
///     .Build();
/// </summary>
public interface IOrderBuilder
{
    #region Required Steps

    /// <summary>
    /// Set User cho đơn hàng (REQUIRED)
    /// </summary>
    /// <param name="user">User đặt hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithUser(User user);

    /// <summary>
    /// Set User ID cho đơn hàng (Alternative)
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithUserId(int userId);

    /// <summary>
    /// Thêm danh sách sản phẩm từ giỏ hàng (REQUIRED)
    /// Tự động tính giá dựa trên Strategy Pattern
    /// </summary>
    /// <param name="cartItems">Danh sách sản phẩm từ giỏ hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithItems(IEnumerable<CartItem> cartItems);

    /// <summary>
    /// Set địa chỉ giao hàng (REQUIRED)
    /// </summary>
    /// <param name="address">Địa chỉ chi tiết</param>
    /// <param name="phone">Số điện thoại liên hệ</param>
    /// <param name="receiverName">Tên người nhận</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithShippingAddress(string address, string phone, string receiverName);

    /// <summary>
    /// Set phương thức thanh toán (REQUIRED)
    /// </summary>
    /// <param name="method">Phương thức thanh toán</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithPaymentMethod(PaymentMethod method);

    #endregion

    #region Optional Steps

    /// <summary>
    /// Áp dụng mã voucher/coupon (OPTIONAL)
    /// Tự động tính giảm giá qua Decorator Pattern
    /// </summary>
    /// <param name="voucherCode">Mã voucher</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithVoucher(string? voucherCode);

    /// <summary>
    /// Thêm ghi chú cho đơn hàng (OPTIONAL)
    /// </summary>
    /// <param name="notes">Ghi chú từ khách hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithNotes(string? notes);

    /// <summary>
    /// Yêu cầu gói quà (OPTIONAL)
    /// </summary>
    /// <param name="giftMessage">Lời nhắn trên thiệp</param>
    /// <param name="giftWrapFee">Phí gói quà</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithGiftWrap(string? giftMessage, decimal giftWrapFee = 0);

    /// <summary>
    /// Set phí ship (OPTIONAL, mặc định tính tự động)
    /// </summary>
    /// <param name="shippingFee">Phí ship</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithShippingFee(decimal shippingFee);

    /// <summary>
    /// Yêu cầu giao hàng nhanh (OPTIONAL)
    /// </summary>
    /// <param name="isExpress">True = giao nhanh</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithExpressDelivery(bool isExpress = true);

    #endregion

    #region Build

    /// <summary>
    /// Xây dựng và trả về Order hoàn chỉnh
    /// 
    /// Validate:
    /// - Phải có User
    /// - Phải có ít nhất 1 sản phẩm
    /// - Phải có địa chỉ giao hàng
    /// - Phải có phương thức thanh toán
    /// 
    /// Tính toán:
    /// - SubTotal từ các OrderItems
    /// - TotalDiscount từ Voucher
    /// - ShippingFee (tự động hoặc custom)
    /// - TotalAmount = SubTotal + ShippingFee - TotalDiscount
    /// </summary>
    /// <returns>Order đã được xây dựng hoàn chỉnh</returns>
    /// <exception cref="InvalidOperationException">Nếu thiếu thông tin bắt buộc</exception>
    Order Build();

    /// <summary>
    /// Kiểm tra xem Builder đã sẵn sàng để Build chưa
    /// </summary>
    /// <returns>True nếu đã đủ thông tin bắt buộc</returns>
    bool CanBuild();

    /// <summary>
    /// Lấy danh sách lỗi validation (nếu có)
    /// </summary>
    /// <returns>Danh sách lỗi</returns>
    IReadOnlyList<string> GetValidationErrors();

    /// <summary>
    /// Reset builder về trạng thái ban đầu
    /// </summary>
    /// <returns>Builder đã reset</returns>
    IOrderBuilder Reset();

    #endregion
}

/// <summary>
/// DTO đại diện cho item trong giỏ hàng
/// Dùng để truyền vào Builder
/// </summary>
public class CartItem
{
    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Sản phẩm (optional, có thể load từ DB)
    /// </summary>
    public Product? Product { get; set; }
}

/// <summary>
/// Kết quả build từ OrderBuilder
/// Chứa thông tin chi tiết về quá trình build
/// </summary>
public class OrderBuildResult
{
    /// <summary>
    /// Order đã được xây dựng
    /// </summary>
    public Order Order { get; set; } = null!;

    /// <summary>
    /// Tổng tiền gốc (chưa giảm)
    /// </summary>
    public decimal OriginalTotal { get; set; }

    /// <summary>
    /// Tổng giảm giá
    /// </summary>
    public decimal TotalDiscount { get; set; }

    /// <summary>
    /// Chi tiết các khoản giảm giá
    /// </summary>
    public List<DiscountDetail> AppliedDiscounts { get; set; } = new();

    /// <summary>
    /// Cảnh báo (ví dụ: sản phẩm sắp hết hạn)
    /// </summary>
    public List<string> Warnings { get; set; } = new();

    /// <summary>
    /// Voucher code đã áp dụng
    /// </summary>
    public string? AppliedVoucherCode { get; set; }

    /// <summary>
    /// Có gói quà không
    /// </summary>
    public bool HasGiftWrap { get; set; }

    /// <summary>
    /// Có giao nhanh không
    /// </summary>
    public bool IsExpressDelivery { get; set; }
}

/// <summary>
/// Chi tiết một khoản giảm giá
/// </summary>
public class DiscountDetail
{
    /// <summary>
    /// Tên loại giảm giá (VIP, Expiry, FlashSale, Voucher)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Mô tả
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền giảm
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Phần trăm giảm (nếu có)
    /// </summary>
    public decimal? Percent { get; set; }
}


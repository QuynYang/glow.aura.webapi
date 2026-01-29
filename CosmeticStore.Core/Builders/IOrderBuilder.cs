using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Builders;


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

public interface IOrderBuilder
{
    #region Required Steps

    
    /// Set User cho đơn hàng (REQUIRED)
    
    /// <param name="user">User đặt hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithUser(User user);

    
    /// Set User ID cho đơn hàng (Alternative)
    
    /// <param name="userId">User ID</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithUserId(int userId);

    
    /// Thêm danh sách sản phẩm từ giỏ hàng (REQUIRED)
    /// Tự động tính giá dựa trên Strategy Pattern
    
    /// <param name="cartItems">Danh sách sản phẩm từ giỏ hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithItems(IEnumerable<CartItem> cartItems);

    
    /// Set địa chỉ giao hàng (REQUIRED)
    
    /// <param name="address">Địa chỉ chi tiết</param>
    /// <param name="phone">Số điện thoại liên hệ</param>
    /// <param name="receiverName">Tên người nhận</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithShippingAddress(string address, string phone, string receiverName);

    
    /// Set phương thức thanh toán (REQUIRED)
    
    /// <param name="method">Phương thức thanh toán</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithPaymentMethod(PaymentMethod method);

    #endregion

    #region Optional Steps

    
    /// Áp dụng mã voucher/coupon (OPTIONAL)
    /// Tự động tính giảm giá qua Decorator Pattern
    
    /// <param name="voucherCode">Mã voucher</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithVoucher(string? voucherCode);

    
    /// Thêm ghi chú cho đơn hàng (OPTIONAL)
    
    /// <param name="notes">Ghi chú từ khách hàng</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithNotes(string? notes);

    
    /// Yêu cầu gói quà (OPTIONAL)
    
    /// <param name="giftMessage">Lời nhắn trên thiệp</param>
    /// <param name="giftWrapFee">Phí gói quà</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithGiftWrap(string? giftMessage, decimal giftWrapFee = 0);

    
    /// Set phí ship (OPTIONAL, mặc định tính tự động)
    
    /// <param name="shippingFee">Phí ship</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithShippingFee(decimal shippingFee);

    
    /// Yêu cầu giao hàng nhanh (OPTIONAL)
    
    /// <param name="isExpress">True = giao nhanh</param>
    /// <returns>Builder để tiếp tục nối chuỗi</returns>
    IOrderBuilder WithExpressDelivery(bool isExpress = true);

    #endregion

    #region Build

    
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
    
    /// <returns>Order đã được xây dựng hoàn chỉnh</returns>
    /// <exception cref="InvalidOperationException">Nếu thiếu thông tin bắt buộc</exception>
    Order Build();

    
    /// Kiểm tra xem Builder đã sẵn sàng để Build chưa
    
    /// <returns>True nếu đã đủ thông tin bắt buộc</returns>
    bool CanBuild();

    
    /// Lấy danh sách lỗi validation (nếu có)
    
    /// <returns>Danh sách lỗi</returns>
    IReadOnlyList<string> GetValidationErrors();

    
    /// Reset builder về trạng thái ban đầu
    
    /// <returns>Builder đã reset</returns>
    IOrderBuilder Reset();

    #endregion
}


/// DTO đại diện cho item trong giỏ hàng
/// Dùng để truyền vào Builder

public class CartItem
{
    
    /// ID sản phẩm
    
    public int ProductId { get; set; }

    
    /// Số lượng
    
    public int Quantity { get; set; }

    
    /// Sản phẩm (optional, có thể load từ DB)
    
    public Product? Product { get; set; }
}


/// Kết quả build từ OrderBuilder
/// Chứa thông tin chi tiết về quá trình build

public class OrderBuildResult
{
    
    /// Order đã được xây dựng
    
    public Order Order { get; set; } = null!;

    
    /// Tổng tiền gốc (chưa giảm)
    
    public decimal OriginalTotal { get; set; }

    
    /// Tổng giảm giá
    
    public decimal TotalDiscount { get; set; }

    
    /// Chi tiết các khoản giảm giá
    
    public List<DiscountDetail> AppliedDiscounts { get; set; } = new();

    
    /// Cảnh báo (ví dụ: sản phẩm sắp hết hạn)
    
    public List<string> Warnings { get; set; } = new();

    
    /// Voucher code đã áp dụng
    
    public string? AppliedVoucherCode { get; set; }

    
    /// Có gói quà không
    
    public bool HasGiftWrap { get; set; }

    
    /// Có giao nhanh không
    
    public bool IsExpressDelivery { get; set; }
}


/// Chi tiết một khoản giảm giá

public class DiscountDetail
{
    
    /// Tên loại giảm giá (VIP, Expiry, FlashSale, Voucher)
    
    public string Type { get; set; } = string.Empty;

    
    /// Mô tả
    
    public string Description { get; set; } = string.Empty;

    
    /// Số tiền giảm
    
    public decimal Amount { get; set; }

    
    /// Phần trăm giảm (nếu có)
    
    public decimal? Percent { get; set; }
}


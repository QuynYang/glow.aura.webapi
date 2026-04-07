using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;


/// FACADE PATTERN - Giao diện đơn giản hóa cho quy trình Checkout

/// MỤC ĐÍCH:
/// - Ẩn sự phức tạp của nhiều hệ thống con (Pricing, Order, Payment, Notification)
/// - Cung cấp 1 method duy nhất để thực hiện toàn bộ quy trình thanh toán
/// - Client (Controller) chỉ cần gọi 1 lần thay vì gọi nhiều service
public interface ICheckoutFacade
{
    
    /// Xử lý toàn bộ quy trình checkout trong 1 lần gọi
    /// Bao gồm: Tính giá → Tạo đơn → Xử lý thanh toán → Gửi thông báo
    
    Task<CheckoutResult> ProcessCheckoutAsync(CheckoutRequest request);

    
    /// Xem trước đơn hàng trước khi checkout (không tạo đơn thật)
    /// Chỉ tính giá và hiển thị tổng tiền
    
    Task<CheckoutPreview> PreviewCheckoutAsync(CheckoutRequest request);
}


/// Thông tin yêu cầu checkout từ khách hàng

public class CheckoutRequest
{
    
    /// ID người dùng
    
    public int UserId { get; set; }

    
    /// Danh sách sản phẩm và số lượng
    
    public List<CheckoutItem> Items { get; set; } = new();

    
    /// Địa chỉ giao hàng
    
    public string ShippingAddress { get; set; } = string.Empty;

    
    /// Số điện thoại nhận hàng
    
    public string ShippingPhone { get; set; } = string.Empty;

    
    /// Tên người nhận
    
    public string ReceiverName { get; set; } = string.Empty;

    
    /// Phương thức thanh toán (COD, MOMO, VNPAY, ZALOPAY)
    
    public string PaymentMethod { get; set; } = "COD";

    
    /// Mã giảm giá (nếu có)
    
    public string? CouponCode { get; set; }

    
    /// Ghi chú đơn hàng
    
    public string? Notes { get; set; }
}


/// Sản phẩm trong giỏ hàng checkout

public class CheckoutItem
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}


/// Kết quả sau khi checkout thành công

public class CheckoutResult
{
    
    /// Checkout có thành công không
    
    public bool IsSuccess { get; set; }

    
    /// Thông báo kết quả
    
    public string Message { get; set; } = string.Empty;

    
    /// ID đơn hàng đã tạo
    
    public int? OrderId { get; set; }

    
    /// Mã đơn hàng
    
    public string? OrderNumber { get; set; }

    
    /// Tổng tiền thanh toán
    
    public decimal? TotalAmount { get; set; }

    
    /// URL thanh toán (nếu thanh toán online)
    
    public string? PaymentUrl { get; set; }

    
    /// Mã giao dịch thanh toán
    
    public string? TransactionId { get; set; }

    
    /// Chi tiết giá từng sản phẩm
    
    public List<CheckoutItemDetail> ItemDetails { get; set; } = new();
}


/// Chi tiết giá của từng sản phẩm trong checkout

public class CheckoutItemDetail
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal OriginalPrice { get; set; }
    public decimal FinalPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public List<string> AppliedDiscounts { get; set; } = new();
}


/// Bản xem trước checkout (chưa tạo đơn thật)

public class CheckoutPreview
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    public decimal SubTotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal TotalDiscount { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CheckoutItemDetail> ItemDetails { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}

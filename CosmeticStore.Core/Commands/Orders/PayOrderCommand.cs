using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Commands.Orders;

/// <summary>
/// Command thanh toán đơn hàng
/// 
/// COMMAND PATTERN + FACTORY PATTERN:
/// - Command chứa thông tin thanh toán
/// - Handler sử dụng PaymentFactory để tạo đúng Payment Service
/// 
/// Business Rules:
/// - Chỉ thanh toán được đơn hàng Confirmed hoặc PaymentFailed
/// - Gọi đến cổng thanh toán tương ứng
/// - Retry nếu thất bại
/// </summary>
public class PayOrderCommand : CommandBase<PayOrderResult>
{
    /// <summary>
    /// ID đơn hàng cần thanh toán
    /// </summary>
    public int OrderId { get; }

    /// <summary>
    /// Phương thức thanh toán
    /// </summary>
    public PaymentMethod PaymentMethod { get; }

    /// <summary>
    /// URL callback sau thanh toán
    /// </summary>
    public string? ReturnUrl { get; }

    /// <summary>
    /// URL khi hủy thanh toán
    /// </summary>
    public string? CancelUrl { get; }

    /// <summary>
    /// Thông tin bổ sung (ví dụ: số điện thoại Momo)
    /// </summary>
    public Dictionary<string, string>? Metadata { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public PayOrderCommand(
        int orderId,
        PaymentMethod paymentMethod,
        string? returnUrl = null,
        string? cancelUrl = null,
        Dictionary<string, string>? metadata = null)
    {
        OrderId = orderId;
        PaymentMethod = paymentMethod;
        ReturnUrl = returnUrl;
        CancelUrl = cancelUrl;
        Metadata = metadata;
    }
}

/// <summary>
/// Kết quả thanh toán
/// </summary>
public class PayOrderResult
{
    /// <summary>
    /// Thanh toán thành công chưa
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Mã giao dịch từ cổng thanh toán
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// URL redirect đến cổng thanh toán (cho thanh toán online)
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// QR Code (cho Momo, VNPay)
    /// </summary>
    public string? QrCode { get; set; }

    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Phương thức thanh toán
    /// </summary>
    public PaymentMethod PaymentMethod { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian hết hạn thanh toán (cho online payment)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}


namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho Payment Gateway - Cổng thanh toán
/// 
/// FACTORY PATTERN:
/// - Interface chung cho tất cả cổng thanh toán
/// - PaymentFactory sẽ trả về đúng Gateway dựa trên phương thức
/// 
/// ĐA HÌNH (Polymorphism):
/// - MomoGateway, ZaloPayGateway, CODGateway đều implement interface này
/// - Client chỉ làm việc với IPaymentGateway, không biết class cụ thể
/// 
/// Workflow:
/// 1. Client gọi PaymentFactory.CreateGateway("MOMO")
/// 2. Factory trả về MomoGateway (as IPaymentGateway)
/// 3. Client gọi ProcessPayment() mà không biết chi tiết bên trong
/// </summary>
public interface IPaymentGateway
{
    /// <summary>
    /// Mã cổng thanh toán (MOMO, ZALOPAY, VNPAY, COD...)
    /// </summary>
    string GatewayCode { get; }

    /// <summary>
    /// Tên hiển thị
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Logo URL
    /// </summary>
    string? LogoUrl { get; }

    /// <summary>
    /// Có yêu cầu thanh toán online không
    /// </summary>
    bool RequiresOnlinePayment { get; }

    /// <summary>
    /// Xử lý thanh toán
    /// </summary>
    /// <param name="request">Thông tin thanh toán</param>
    /// <returns>Kết quả thanh toán</returns>
    Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentRequest request);

    /// <summary>
    /// Kiểm tra trạng thái giao dịch
    /// </summary>
    /// <param name="transactionId">Mã giao dịch</param>
    /// <returns>Trạng thái giao dịch</returns>
    Task<PaymentGatewayResult> QueryTransactionAsync(string transactionId);

    /// <summary>
    /// Hoàn tiền giao dịch
    /// </summary>
    /// <param name="transactionId">Mã giao dịch gốc</param>
    /// <param name="amount">Số tiền hoàn</param>
    /// <param name="reason">Lý do hoàn tiền</param>
    /// <returns>Kết quả hoàn tiền</returns>
    Task<PaymentGatewayResult> RefundAsync(string transactionId, decimal amount, string? reason = null);

    /// <summary>
    /// Xác thực callback/webhook từ cổng thanh toán
    /// </summary>
    /// <param name="payload">Dữ liệu callback</param>
    /// <param name="signature">Chữ ký</param>
    /// <returns>Dữ liệu đã xác thực</returns>
    Task<PaymentCallbackResult> VerifyCallbackAsync(string payload, string signature);
}

/// <summary>
/// Request thanh toán
/// </summary>
public class PaymentRequest
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string OrderId { get; set; } = string.Empty;

    /// <summary>
    /// Mã đơn hàng nội bộ
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Số tiền thanh toán
    /// </summary>
    public decimal Amount { get; set; }

    /// <summary>
    /// Mô tả giao dịch
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// URL callback sau thanh toán thành công
    /// </summary>
    public string? ReturnUrl { get; set; }

    /// <summary>
    /// URL callback khi hủy thanh toán
    /// </summary>
    public string? CancelUrl { get; set; }

    /// <summary>
    /// URL nhận thông báo từ cổng thanh toán (IPN/Webhook)
    /// </summary>
    public string? NotifyUrl { get; set; }

    /// <summary>
    /// Thông tin khách hàng
    /// </summary>
    public CustomerInfo? Customer { get; set; }

    /// <summary>
    /// Dữ liệu bổ sung
    /// </summary>
    public Dictionary<string, string>? ExtraData { get; set; }
}

/// <summary>
/// Thông tin khách hàng
/// </summary>
public class CustomerInfo
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
}

/// <summary>
/// Kết quả từ Payment Gateway
/// </summary>
public class PaymentGatewayResult
{
    /// <summary>
    /// Thành công hay không
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Mã giao dịch từ cổng thanh toán
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Mã tham chiếu
    /// </summary>
    public string? ReferenceId { get; set; }

    /// <summary>
    /// URL redirect đến cổng thanh toán
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// QR Code để quét thanh toán
    /// </summary>
    public string? QrCodeUrl { get; set; }

    /// <summary>
    /// Nội dung QR Code
    /// </summary>
    public string? QrCodeData { get; set; }

    /// <summary>
    /// Deep link (mở app thanh toán)
    /// </summary>
    public string? DeepLink { get; set; }

    /// <summary>
    /// Trạng thái giao dịch
    /// </summary>
    public PaymentStatus Status { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Mã lỗi (nếu có)
    /// </summary>
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Thời gian hết hạn thanh toán
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Dữ liệu thô từ cổng thanh toán
    /// </summary>
    public string? RawResponse { get; set; }

    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static PaymentGatewayResult Success(
        string transactionId, 
        string? paymentUrl = null, 
        string? qrCodeData = null,
        string? deepLink = null)
    {
        return new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            PaymentUrl = paymentUrl,
            QrCodeData = qrCodeData,
            DeepLink = deepLink,
            Status = PaymentStatus.Pending,
            Message = "Giao dịch được tạo thành công"
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại
    /// </summary>
    public static PaymentGatewayResult Failure(string errorMessage, string? errorCode = null)
    {
        return new PaymentGatewayResult
        {
            IsSuccess = false,
            Status = PaymentStatus.Failed,
            Message = errorMessage,
            ErrorCode = errorCode
        };
    }
}

/// <summary>
/// Kết quả callback từ cổng thanh toán
/// </summary>
public class PaymentCallbackResult
{
    public bool IsValid { get; set; }
    public string? TransactionId { get; set; }
    public string? OrderId { get; set; }
    public decimal Amount { get; set; }
    public PaymentStatus Status { get; set; }
    public string? Message { get; set; }
}

/// <summary>
/// Trạng thái thanh toán
/// </summary>
public enum PaymentStatus
{
    /// <summary>
    /// Chờ thanh toán
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đang xử lý
    /// </summary>
    Processing = 1,

    /// <summary>
    /// Thành công
    /// </summary>
    Success = 2,

    /// <summary>
    /// Thất bại
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Đã hủy
    /// </summary>
    Cancelled = 4,

    /// <summary>
    /// Đã hoàn tiền
    /// </summary>
    Refunded = 5,

    /// <summary>
    /// Hết hạn
    /// </summary>
    Expired = 6
}


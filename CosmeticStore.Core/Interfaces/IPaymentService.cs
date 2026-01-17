namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho các dịch vụ thanh toán - Áp dụng tính TRỪU TƯỢNG (Abstraction)
/// 
/// Kết hợp với Factory Pattern để tạo đúng loại Payment Service
/// dựa trên phương thức thanh toán mà khách hàng chọn.
/// 
/// Các implementation:
/// - MomoPaymentService: Thanh toán qua ví Momo
/// - CodPaymentService: Thanh toán khi nhận hàng (COD)
/// - VnPayPaymentService: Thanh toán qua VNPay
/// - ZaloPayPaymentService: Thanh toán qua ZaloPay
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Xử lý thanh toán
    /// </summary>
    /// <param name="amount">Số tiền thanh toán</param>
    /// <param name="orderId">Mã đơn hàng</param>
    /// <param name="description">Mô tả giao dịch</param>
    /// <returns>Kết quả thanh toán</returns>
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId, string? description = null);

    /// <summary>
    /// Tên phương thức thanh toán
    /// </summary>
    string PaymentMethod { get; }

    /// <summary>
    /// Kiểm tra trạng thái giao dịch
    /// </summary>
    Task<PaymentResult> CheckTransactionStatusAsync(string transactionId);

    /// <summary>
    /// Hoàn tiền
    /// </summary>
    Task<PaymentResult> RefundAsync(string transactionId, decimal amount, string? reason = null);
}

/// <summary>
/// Kết quả thanh toán
/// </summary>
public class PaymentResult
{
    /// <summary>
    /// Thanh toán thành công hay không
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Thông báo lỗi (nếu có)
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// URL redirect đến cổng thanh toán
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// QR Code để quét thanh toán
    /// </summary>
    public string? QrCode { get; set; }

    /// <summary>
    /// Mã giao dịch từ cổng thanh toán
    /// </summary>
    public string? TransactionId { get; set; }

    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static PaymentResult Success(string transactionId, string? paymentUrl = null, string? qrCode = null)
    {
        return new PaymentResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            PaymentUrl = paymentUrl,
            QrCode = qrCode,
            Message = "Thanh toán thành công"
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại
    /// </summary>
    public static PaymentResult Failure(string errorMessage)
    {
        return new PaymentResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            Message = "Thanh toán thất bại"
        };
    }
}


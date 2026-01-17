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
/// </summary>
public interface IPaymentService
{
    /// <summary>
    /// Xử lý thanh toán
    /// </summary>
    /// <param name="amount">Số tiền thanh toán</param>
    /// <param name="orderId">Mã đơn hàng</param>
    /// <returns>Kết quả thanh toán (URL redirect hoặc thông báo)</returns>
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId);

    /// <summary>
    /// Tên phương thức thanh toán
    /// </summary>
    string PaymentMethod { get; }
}

/// <summary>
/// Kết quả thanh toán
/// </summary>
public class PaymentResult
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }
    public string? RedirectUrl { get; set; }
    public string? TransactionId { get; set; }
}



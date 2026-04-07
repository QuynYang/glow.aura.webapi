using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;


/// FACADE PATTERN - Giao diện đơn giản hóa cho quy trình thanh toán đơn hàng

public interface IPaymentIntegrationFacade
{
    
    /// Xử lý toàn bộ quy trình thanh toán trong 1 lần gọi
    ///
    /// Bao gồm: Lấy đơn hàng → Kiểm tra trạng thái → Gọi cổng thanh toán
    /// → Sinh mã QR / URL chuyển hướng → Cập nhật đơn hàng → Ghi log
    
    /// <param name="orderId">ID đơn hàng cần thanh toán</param>
    /// <param name="paymentMethod">Phương thức thanh toán (Momo, VNPay, ZaloPay, COD)</param>
    /// <param name="returnUrl">URL chuyển hướng sau khi thanh toán (tùy chọn)</param>
    /// <returns>Kết quả thanh toán bao gồm URL, QR code, transaction ID</returns>
    Task<PaymentFacadeResult> ProcessPaymentAsync(
        int orderId,
        PaymentMethod paymentMethod,
        string? returnUrl = null);
}


/// Kết quả trả về từ PaymentIntegrationFacade
/// Gom tất cả thông tin thanh toán vào 1 object duy nhất

public class PaymentFacadeResult
{
    
    /// Thanh toán có thành công không
    
    public bool IsSuccess { get; set; }

    
    /// Thông báo kết quả (hiển thị cho khách)
    
    public string Message { get; set; } = string.Empty;

    
    /// Mã lỗi (nếu có) - dùng cho hệ thống
    
    public string? ErrorCode { get; set; }

    
    /// Mã giao dịch từ cổng thanh toán
    
    public string? TransactionId { get; set; }

    
    /// URL chuyển hướng thanh toán (Momo, VNPay, ZaloPay)
    
    public string? PaymentUrl { get; set; }

    
    /// Dữ liệu mã QR (cho Momo, ZaloPay)
    
    public string? QrCodeData { get; set; }

    
    /// Tổng tiền thanh toán
    
    public decimal Amount { get; set; }

    
    /// Phương thức thanh toán đã dùng
    
    public PaymentMethod PaymentMethod { get; set; }

    
    /// Thời gian hết hạn thanh toán (nếu có)
    
    public DateTime? ExpiresAt { get; set; }

    
    /// Tạo kết quả thất bại
    
    public static PaymentFacadeResult Fail(string message, string? errorCode = null)
    {
        return new PaymentFacadeResult
        {
            IsSuccess = false,
            Message = message,
            ErrorCode = errorCode
        };
    }
}

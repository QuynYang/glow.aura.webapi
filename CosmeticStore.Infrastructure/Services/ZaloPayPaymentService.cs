using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Dịch vụ thanh toán qua ZaloPay
/// Implement IPaymentService interface (Abstraction)
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentFactory
/// - Xử lý thanh toán qua ví ZaloPay
/// </summary>
public class ZaloPayPaymentService : IPaymentService
{
    public string PaymentMethod => "ZALOPAY";

    /// <summary>
    /// Xử lý thanh toán qua ZaloPay
    /// (Đây là code mẫu - thực tế cần tích hợp API ZaloPay)
    /// </summary>
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId, string? description = null)
    {
        await Task.Delay(100); // Giả lập gọi API

        var transactionId = $"ZALOPAY-{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        
        // TODO: Tích hợp ZaloPay API thực tế
        // 1. Tạo order với ZaloPay
        // 2. Nhận QR code hoặc deeplink
        // 3. Xử lý callback

        return PaymentResult.Success(
            transactionId: transactionId,
            paymentUrl: $"https://zalopay.vn/payment?apptransid={transactionId}",
            qrCode: $"00020101021238620010A00000072701320006970436010{transactionId}"
        );
    }

    /// <summary>
    /// Kiểm tra trạng thái giao dịch
    /// </summary>
    public async Task<PaymentResult> CheckTransactionStatusAsync(string transactionId)
    {
        await Task.Delay(50);
        
        return new PaymentResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Message = "Giao dịch ZaloPay thành công"
        };
    }

    /// <summary>
    /// Hoàn tiền qua ZaloPay
    /// </summary>
    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(100);
        
        return PaymentResult.Success(
            transactionId: $"REFUND-{transactionId}"
        );
    }
}


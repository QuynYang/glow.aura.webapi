using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Dịch vụ thanh toán qua VNPay
/// Implement IPaymentService interface (Abstraction)
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentFactory
/// - Xử lý thanh toán qua cổng VNPay
/// </summary>
public class VnPayPaymentService : IPaymentService
{
    public string PaymentMethod => "VNPAY";

    /// <summary>
    /// Xử lý thanh toán qua VNPay
    /// (Đây là code mẫu - thực tế cần tích hợp API VNPay)
    /// </summary>
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId, string? description = null)
    {
        await Task.Delay(100); // Giả lập gọi API

        var transactionId = $"VNPAY-{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}";
        
        // TODO: Tích hợp VNPay API thực tế
        // 1. Tạo request với chữ ký số
        // 2. Redirect đến VNPay
        // 3. Xử lý callback IPN

        return PaymentResult.Success(
            transactionId: transactionId,
            paymentUrl: $"https://sandbox.vnpayment.vn/paymentv2/vpcpay.html?vnp_TxnRef={orderId}&vnp_Amount={amount * 100}"
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
            Message = "Giao dịch VNPay thành công"
        };
    }

    /// <summary>
    /// Hoàn tiền qua VNPay
    /// </summary>
    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(100);
        
        return PaymentResult.Success(
            transactionId: $"REFUND-{transactionId}"
        );
    }
}


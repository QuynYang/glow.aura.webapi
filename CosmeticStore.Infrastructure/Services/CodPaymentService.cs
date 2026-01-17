using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Dịch vụ thanh toán khi nhận hàng (Cash On Delivery)
/// Implement IPaymentService interface (Abstraction)
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentFactory
/// - Xử lý đơn hàng thanh toán khi nhận
/// </summary>
public class CodPaymentService : IPaymentService
{
    public string PaymentMethod => "COD";

    /// <summary>
    /// Xử lý đơn hàng thanh toán khi nhận
    /// </summary>
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId, string? description = null)
    {
        await Task.Delay(50); // Giả lập xử lý

        return PaymentResult.Success(
            transactionId: $"COD-{orderId}-{DateTime.UtcNow:yyyyMMddHHmmss}"
        );
    }

    /// <summary>
    /// Kiểm tra trạng thái giao dịch (COD luôn chờ giao hàng)
    /// </summary>
    public async Task<PaymentResult> CheckTransactionStatusAsync(string transactionId)
    {
        await Task.Delay(10);
        
        return new PaymentResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Message = "Đơn hàng COD - Thanh toán khi nhận hàng"
        };
    }

    /// <summary>
    /// Hoàn tiền COD (không áp dụng - chưa thu tiền)
    /// </summary>
    public async Task<PaymentResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(10);
        
        return PaymentResult.Failure("COD không hỗ trợ hoàn tiền - Đơn hàng chưa thu tiền");
    }
}

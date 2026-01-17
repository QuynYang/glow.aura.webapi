using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Gateways;

/// <summary>
/// COD (Cash On Delivery) Gateway - Thanh toán khi nhận hàng
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentGatewayFactory
/// - Implement IPaymentGateway interface
/// 
/// Đặc điểm:
/// - Không cần thanh toán online
/// - Không có redirect URL
/// - Giao dịch hoàn thành khi shipper thu tiền
/// </summary>
public class CODGateway : IPaymentGateway
{
    public string GatewayCode => "COD";
    public string DisplayName => "Thanh toán khi nhận hàng";
    public string? LogoUrl => null;
    public bool RequiresOnlinePayment => false;

    /// <summary>
    /// Xử lý đơn hàng COD
    /// COD không cần thanh toán online, chỉ tạo giao dịch pending
    /// </summary>
    public async Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentRequest request)
    {
        await Task.Delay(10);

        var transactionId = $"COD-{request.OrderNumber}-{DateTime.UtcNow:yyyyMMddHHmmss}";

        return new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Status = PaymentStatus.Pending, // COD luôn pending cho đến khi shipper thu tiền
            Message = $"Đơn hàng {request.OrderNumber} sẽ thanh toán {request.Amount:N0} VND khi nhận hàng",
            ExpiresAt = null // COD không có thời hạn thanh toán
        };
    }

    /// <summary>
    /// Kiểm tra trạng thái - COD trạng thái do hệ thống quản lý
    /// </summary>
    public async Task<PaymentGatewayResult> QueryTransactionAsync(string transactionId)
    {
        await Task.Delay(10);

        return new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Status = PaymentStatus.Pending,
            Message = "Đơn hàng COD - Chờ shipper thu tiền"
        };
    }

    /// <summary>
    /// COD không hỗ trợ hoàn tiền tự động (chưa thu tiền)
    /// </summary>
    public async Task<PaymentGatewayResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(10);

        return PaymentGatewayResult.Failure(
            "COD không hỗ trợ hoàn tiền - Đơn hàng chưa thu tiền",
            "COD_REFUND_NOT_SUPPORTED"
        );
    }

    /// <summary>
    /// COD không có callback từ cổng thanh toán
    /// </summary>
    public async Task<PaymentCallbackResult> VerifyCallbackAsync(string payload, string signature)
    {
        await Task.Delay(10);

        return new PaymentCallbackResult
        {
            IsValid = false,
            Message = "COD không hỗ trợ callback"
        };
    }

    /// <summary>
    /// Xác nhận shipper đã thu tiền COD
    /// </summary>
    public Task<PaymentGatewayResult> ConfirmCollectedAsync(string transactionId, decimal collectedAmount)
    {
        return Task.FromResult(new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Status = PaymentStatus.Success,
            Message = $"Đã thu tiền COD: {collectedAmount:N0} VND"
        });
    }
}


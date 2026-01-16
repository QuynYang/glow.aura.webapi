using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Dịch vụ thanh toán khi nhận hàng (Cash On Delivery)
/// Implement IPaymentService interface (Abstraction)
/// </summary>
public class CodPaymentService : IPaymentService
{
    public string PaymentMethod => "COD";

    /// <summary>
    /// Xử lý đơn hàng thanh toán khi nhận
    /// </summary>
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId)
    {
        await Task.Delay(50); // Giả lập xử lý

        return new PaymentResult
        {
            IsSuccess = true,
            Message = $"Đơn hàng {orderId} sẽ thanh toán {amount:N0} VND khi nhận hàng",
            TransactionId = $"COD-{orderId}"
        };
    }
}


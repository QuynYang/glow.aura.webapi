using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Dịch vụ thanh toán qua ví Momo
/// Implement IPaymentService interface (Abstraction)
/// </summary>
public class MomoPaymentService : IPaymentService
{
    public string PaymentMethod => "MOMO";

    /// <summary>
    /// Xử lý thanh toán qua Momo
    /// (Đây là code mẫu - thực tế cần tích hợp API Momo)
    /// </summary>
    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, string orderId)
    {
        // TODO: Tích hợp API Momo thực tế
        // 1. Tạo request đến Momo API
        // 2. Nhận URL redirect để khách thanh toán
        // 3. Xử lý callback từ Momo
        
        await Task.Delay(100); // Giả lập gọi API

        return new PaymentResult
        {
            IsSuccess = true,
            Message = "Vui lòng thanh toán qua ứng dụng Momo",
            RedirectUrl = $"https://momo.vn/payment?orderId={orderId}&amount={amount}",
            TransactionId = Guid.NewGuid().ToString()
        };
    }
}



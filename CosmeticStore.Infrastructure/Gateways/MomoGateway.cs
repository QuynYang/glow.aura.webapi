using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Gateways;

/// <summary>
/// Momo Payment Gateway
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentGatewayFactory
/// - Implement IPaymentGateway interface
/// 
/// ĐA HÌNH (Polymorphism):
/// - Client gọi ProcessPaymentAsync() mà không biết đây là Momo
/// - Có thể thay thế bằng ZaloPayGateway, VNPayGateway...
/// 
/// Tích hợp Momo API:
/// - Sandbox: https://test-payment.momo.vn
/// - Production: https://payment.momo.vn
/// </summary>
public class MomoGateway : IPaymentGateway
{
    // Cấu hình Momo (thực tế lấy từ appsettings.json)
    private readonly string _partnerCode = "MOMO_PARTNER";
    private readonly string _accessKey = "F8BBA842ECF85";
    private readonly string _secretKey = "K951B6PE1waDMi640xX08PD3vg6EkVlz";
    private readonly string _endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";

    public string GatewayCode => "MOMO";
    public string DisplayName => "Ví Momo";
    public string? LogoUrl => "https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png";
    public bool RequiresOnlinePayment => true;

    /// <summary>
    /// Xử lý thanh toán qua Momo
    /// </summary>
    public async Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            // Tạo request ID
            var requestId = Guid.NewGuid().ToString();
            var orderId = $"MOMO{request.OrderNumber}{DateTime.UtcNow:yyyyMMddHHmmss}";

            // Tạo chữ ký HMAC SHA256
            var rawSignature = $"accessKey={_accessKey}" +
                              $"&amount={request.Amount:0}" +
                              $"&extraData={request.ExtraData?.GetValueOrDefault("data", "")}" +
                              $"&ipnUrl={request.NotifyUrl}" +
                              $"&orderId={orderId}" +
                              $"&orderInfo={request.Description}" +
                              $"&partnerCode={_partnerCode}" +
                              $"&redirectUrl={request.ReturnUrl}" +
                              $"&requestId={requestId}" +
                              $"&requestType=captureWallet";

            var signature = ComputeHmacSha256(rawSignature, _secretKey);

            // TODO: Gọi Momo API thực tế
            // var response = await _httpClient.PostAsJsonAsync(_endpoint, momoRequest);

            await Task.Delay(100); // Giả lập gọi API

            // Giả lập response thành công
            return PaymentGatewayResult.Success(
                transactionId: orderId,
                paymentUrl: $"https://test-payment.momo.vn/pay?orderId={orderId}",
                qrCodeData: $"00020101021238570010A000000727012600069704220108{orderId}",
                deepLink: $"momo://app?action=pay&orderId={orderId}"
            );
        }
        catch (Exception ex)
        {
            return PaymentGatewayResult.Failure($"Lỗi kết nối Momo: {ex.Message}", "MOMO_ERROR");
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái giao dịch
    /// </summary>
    public async Task<PaymentGatewayResult> QueryTransactionAsync(string transactionId)
    {
        await Task.Delay(50);

        // TODO: Gọi Momo API query transaction
        return new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Status = PaymentStatus.Success,
            Message = "Giao dịch thành công"
        };
    }

    /// <summary>
    /// Hoàn tiền
    /// </summary>
    public async Task<PaymentGatewayResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(100);

        // TODO: Gọi Momo API refund
        return PaymentGatewayResult.Success(
            transactionId: $"REFUND-{transactionId}-{DateTime.UtcNow:yyyyMMddHHmmss}"
        );
    }

    /// <summary>
    /// Xác thực callback từ Momo
    /// </summary>
    public async Task<PaymentCallbackResult> VerifyCallbackAsync(string payload, string signature)
    {
        await Task.Delay(10);

        try
        {
            // Parse payload
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);
            
            // TODO: Verify signature với Momo secret key
            // var expectedSignature = ComputeHmacSha256(rawData, _secretKey);
            // if (signature != expectedSignature) return invalid

            return new PaymentCallbackResult
            {
                IsValid = true,
                TransactionId = data?["orderId"]?.ToString(),
                OrderId = data?["orderId"]?.ToString(),
                Status = PaymentStatus.Success,
                Message = "Callback hợp lệ"
            };
        }
        catch
        {
            return new PaymentCallbackResult
            {
                IsValid = false,
                Message = "Callback không hợp lệ"
            };
        }
    }

    /// <summary>
    /// Tính HMAC SHA256
    /// </summary>
    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}


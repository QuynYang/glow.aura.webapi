using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Gateways;

/// <summary>
/// ZaloPay Payment Gateway
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentGatewayFactory
/// - Implement IPaymentGateway interface
/// 
/// Tích hợp ZaloPay API:
/// - Sandbox: https://sb-openapi.zalopay.vn/v2
/// - Production: https://openapi.zalopay.vn/v2
/// </summary>
public class ZaloPayGateway : IPaymentGateway
{
    // Cấu hình ZaloPay (thực tế lấy từ appsettings.json)
    private readonly int _appId = 2553;
    private readonly string _key1 = "PcY4iZIKFCIdgZvA6ueMcMHHmpmGPwE1";
    private readonly string _key2 = "kLtgPl8HHhfvMuDHPwKfgfsY4Ydm9eIz";
    private readonly string _endpoint = "https://sb-openapi.zalopay.vn/v2/create";

    public string GatewayCode => "ZALOPAY";
    public string DisplayName => "ZaloPay";
    public string? LogoUrl => "https://upload.wikimedia.org/wikipedia/vi/7/77/ZaloPay_Logo.png";
    public bool RequiresOnlinePayment => true;

    /// <summary>
    /// Xử lý thanh toán qua ZaloPay
    /// </summary>
    public async Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var appTransId = $"{DateTime.UtcNow:yyMMdd}_{request.OrderNumber}_{DateTime.UtcNow:HHmmss}";
            var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Tạo embed_data
            var embedData = JsonSerializer.Serialize(new
            {
                redirecturl = request.ReturnUrl,
                orderId = request.OrderId
            });

            // Tạo chữ ký
            var data = $"{_appId}|{appTransId}|{request.Customer?.Name ?? "Customer"}|{request.Amount:0}|{appTime}|{embedData}|";
            var mac = ComputeHmacSha256(data, _key1);

            // TODO: Gọi ZaloPay API thực tế
            await Task.Delay(100);

            return PaymentGatewayResult.Success(
                transactionId: appTransId,
                paymentUrl: $"https://sbgateway.zalopay.vn/pay?apptransid={appTransId}",
                qrCodeData: $"00020101021238620010A00000072701320006970436010{appTransId}",
                deepLink: $"zalopay://pay?apptransid={appTransId}"
            );
        }
        catch (Exception ex)
        {
            return PaymentGatewayResult.Failure($"Lỗi kết nối ZaloPay: {ex.Message}", "ZALOPAY_ERROR");
        }
    }

    /// <summary>
    /// Kiểm tra trạng thái giao dịch
    /// </summary>
    public async Task<PaymentGatewayResult> QueryTransactionAsync(string transactionId)
    {
        await Task.Delay(50);

        return new PaymentGatewayResult
        {
            IsSuccess = true,
            TransactionId = transactionId,
            Status = PaymentStatus.Success,
            Message = "Giao dịch ZaloPay thành công"
        };
    }

    /// <summary>
    /// Hoàn tiền
    /// </summary>
    public async Task<PaymentGatewayResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(100);

        return PaymentGatewayResult.Success(
            transactionId: $"ZALO-REFUND-{transactionId}-{DateTime.UtcNow:yyyyMMddHHmmss}"
        );
    }

    /// <summary>
    /// Xác thực callback từ ZaloPay
    /// </summary>
    public async Task<PaymentCallbackResult> VerifyCallbackAsync(string payload, string signature)
    {
        await Task.Delay(10);

        try
        {
            // Verify với key2
            var expectedMac = ComputeHmacSha256(payload, _key2);
            
            if (signature != expectedMac)
            {
                return new PaymentCallbackResult
                {
                    IsValid = false,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(payload);

            return new PaymentCallbackResult
            {
                IsValid = true,
                TransactionId = data?["app_trans_id"]?.ToString(),
                OrderId = data?["app_trans_id"]?.ToString(),
                Status = PaymentStatus.Success,
                Message = "Callback ZaloPay hợp lệ"
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

    private static string ComputeHmacSha256(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}


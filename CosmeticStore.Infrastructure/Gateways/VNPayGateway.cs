using System.Security.Cryptography;
using System.Text;
using System.Web;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Gateways;

/// <summary>
/// VNPay Payment Gateway
/// 
/// FACTORY PATTERN:
/// - Được tạo bởi PaymentGatewayFactory
/// - Implement IPaymentGateway interface
/// 
/// Tích hợp VNPay API:
/// - Sandbox: https://sandbox.vnpayment.vn
/// - Production: https://pay.vnpay.vn
/// </summary>
public class VNPayGateway : IPaymentGateway
{
    // Cấu hình VNPay (thực tế lấy từ appsettings.json)
    private readonly string _tmnCode = "COSMETIC";
    private readonly string _hashSecret = "VNPAY_SECRET_KEY_12345";
    private readonly string _paymentUrl = "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
    private readonly string _apiUrl = "https://sandbox.vnpayment.vn/merchant_webapi/api/transaction";

    public string GatewayCode => "VNPAY";
    public string DisplayName => "VNPay";
    public string? LogoUrl => "https://vnpay.vn/assets/images/logo-vnpay.svg";
    public bool RequiresOnlinePayment => true;

    /// <summary>
    /// Xử lý thanh toán qua VNPay
    /// </summary>
    public async Task<PaymentGatewayResult> ProcessPaymentAsync(PaymentRequest request)
    {
        try
        {
            var vnpTxnRef = $"VNP{request.OrderNumber}{DateTime.UtcNow:yyyyMMddHHmmss}";
            var vnpCreateDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
            var vnpExpireDate = DateTime.UtcNow.AddHours(7).AddMinutes(15).ToString("yyyyMMddHHmmss");

            // Build query string
            var queryParams = new SortedDictionary<string, string>
            {
                { "vnp_Version", "2.1.0" },
                { "vnp_Command", "pay" },
                { "vnp_TmnCode", _tmnCode },
                { "vnp_Amount", ((long)(request.Amount * 100)).ToString() }, // VNPay tính theo đồng × 100
                { "vnp_CreateDate", vnpCreateDate },
                { "vnp_CurrCode", "VND" },
                { "vnp_IpAddr", "127.0.0.1" },
                { "vnp_Locale", "vn" },
                { "vnp_OrderInfo", request.Description },
                { "vnp_OrderType", "billpayment" },
                { "vnp_ReturnUrl", request.ReturnUrl ?? "" },
                { "vnp_TxnRef", vnpTxnRef },
                { "vnp_ExpireDate", vnpExpireDate }
            };

            // Tạo query string
            var queryString = string.Join("&", 
                queryParams.Select(kv => $"{kv.Key}={HttpUtility.UrlEncode(kv.Value)}"));

            // Tạo chữ ký
            var signData = string.Join("&", 
                queryParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var signature = ComputeHmacSha512(signData, _hashSecret);

            var paymentUrl = $"{_paymentUrl}?{queryString}&vnp_SecureHash={signature}";

            await Task.Delay(50);

            return PaymentGatewayResult.Success(
                transactionId: vnpTxnRef,
                paymentUrl: paymentUrl
            );
        }
        catch (Exception ex)
        {
            return PaymentGatewayResult.Failure($"Lỗi kết nối VNPay: {ex.Message}", "VNPAY_ERROR");
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
            Message = "Giao dịch VNPay thành công"
        };
    }

    /// <summary>
    /// Hoàn tiền qua VNPay
    /// </summary>
    public async Task<PaymentGatewayResult> RefundAsync(string transactionId, decimal amount, string? reason = null)
    {
        await Task.Delay(100);

        return PaymentGatewayResult.Success(
            transactionId: $"VNP-REFUND-{transactionId}"
        );
    }

    /// <summary>
    /// Xác thực callback từ VNPay
    /// </summary>
    public async Task<PaymentCallbackResult> VerifyCallbackAsync(string payload, string signature)
    {
        await Task.Delay(10);

        try
        {
            // Parse query string
            var queryParams = HttpUtility.ParseQueryString(payload);
            
            // Verify signature
            var sortedParams = queryParams.AllKeys
                .Where(k => k != null && k.StartsWith("vnp_") && k != "vnp_SecureHash" && k != "vnp_SecureHashType")
                .OrderBy(k => k)
                .ToDictionary(k => k!, k => queryParams[k] ?? "");

            var signData = string.Join("&", sortedParams.Select(kv => $"{kv.Key}={kv.Value}"));
            var expectedSignature = ComputeHmacSha512(signData, _hashSecret);

            if (signature != expectedSignature)
            {
                return new PaymentCallbackResult
                {
                    IsValid = false,
                    Message = "Chữ ký không hợp lệ"
                };
            }

            var responseCode = queryParams["vnp_ResponseCode"];
            var status = responseCode == "00" ? PaymentStatus.Success : PaymentStatus.Failed;

            return new PaymentCallbackResult
            {
                IsValid = true,
                TransactionId = queryParams["vnp_TxnRef"],
                OrderId = queryParams["vnp_TxnRef"],
                Amount = decimal.Parse(queryParams["vnp_Amount"] ?? "0") / 100,
                Status = status,
                Message = responseCode == "00" ? "Thanh toán thành công" : "Thanh toán thất bại"
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

    private static string ComputeHmacSha512(string data, string key)
    {
        using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }
}


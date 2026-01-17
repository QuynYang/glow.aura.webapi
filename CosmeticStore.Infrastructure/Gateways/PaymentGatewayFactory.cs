using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Gateways;

/// <summary>
/// Payment Gateway Factory - Factory Pattern
/// 
/// FACTORY PATTERN:
/// - Tạo đúng loại Gateway dựa trên mã thanh toán
/// - Client không cần biết class cụ thể (MomoGateway, ZaloPayGateway...)
/// - Dễ dàng thêm cổng thanh toán mới mà không sửa code cũ
/// 
/// OOP - ĐA HÌNH (Polymorphism):
/// - Trả về IPaymentGateway (interface)
/// - Thực chất là MomoGateway, ZaloPayGateway, VNPayGateway, CODGateway
/// - Client gọi ProcessPaymentAsync() mà không biết đang dùng cổng nào
/// 
/// Ví dụ sử dụng:
/// var gateway = PaymentGatewayFactory.CreateGateway("MOMO");
/// var result = await gateway.ProcessPaymentAsync(request);
/// </summary>
public class PaymentGatewayFactory
{
    /// <summary>
    /// Danh sách Gateway được hỗ trợ (cache để tránh tạo mới)
    /// </summary>
    private static readonly Dictionary<string, Func<IPaymentGateway>> _gateways = new(StringComparer.OrdinalIgnoreCase)
    {
        { "MOMO", () => new MomoGateway() },
        { "ZALOPAY", () => new ZaloPayGateway() },
        { "VNPAY", () => new VNPayGateway() },
        { "COD", () => new CODGateway() },
        { "BANK", () => new VNPayGateway() } // Bank transfer qua VNPay
    };

    /// <summary>
    /// Tạo Payment Gateway dựa trên mã thanh toán
    /// 
    /// FACTORY METHOD:
    /// Input: "MOMO" → Output: new MomoGateway()
    /// Input: "ZALOPAY" → Output: new ZaloPayGateway()
    /// Input: "VNPAY" → Output: new VNPayGateway()
    /// Input: "COD" → Output: new CODGateway()
    /// </summary>
    /// <param name="gatewayCode">Mã cổng thanh toán (MOMO, ZALOPAY, VNPAY, COD)</param>
    /// <returns>IPaymentGateway instance</returns>
    /// <exception cref="ArgumentException">Khi mã không được hỗ trợ</exception>
    public IPaymentGateway CreateGateway(string gatewayCode)
    {
        if (string.IsNullOrWhiteSpace(gatewayCode))
        {
            throw new ArgumentException("Mã cổng thanh toán không được để trống", nameof(gatewayCode));
        }

        if (_gateways.TryGetValue(gatewayCode.ToUpper(), out var factory))
        {
            return factory();
        }

        throw new ArgumentException(
            $"Cổng thanh toán '{gatewayCode}' không được hỗ trợ. " +
            $"Các cổng hỗ trợ: {string.Join(", ", GetSupportedGateways())}",
            nameof(gatewayCode));
    }

    /// <summary>
    /// Kiểm tra xem gateway có được hỗ trợ không
    /// </summary>
    public bool IsSupported(string gatewayCode)
    {
        return !string.IsNullOrWhiteSpace(gatewayCode) && 
               _gateways.ContainsKey(gatewayCode.ToUpper());
    }

    /// <summary>
    /// Lấy danh sách các cổng thanh toán được hỗ trợ
    /// </summary>
    public IEnumerable<string> GetSupportedGateways()
    {
        return _gateways.Keys;
    }

    /// <summary>
    /// Lấy thông tin chi tiết các cổng thanh toán
    /// </summary>
    public IEnumerable<PaymentGatewayInfo> GetGatewayInfos()
    {
        return new[]
        {
            new PaymentGatewayInfo("COD", "Thanh toán khi nhận hàng", null, false),
            new PaymentGatewayInfo("MOMO", "Ví Momo", "https://upload.wikimedia.org/wikipedia/vi/f/fe/MoMo_Logo.png", true),
            new PaymentGatewayInfo("ZALOPAY", "ZaloPay", "https://upload.wikimedia.org/wikipedia/vi/7/77/ZaloPay_Logo.png", true),
            new PaymentGatewayInfo("VNPAY", "VNPay", "https://vnpay.vn/assets/images/logo-vnpay.svg", true),
            new PaymentGatewayInfo("BANK", "Chuyển khoản ngân hàng", null, true)
        };
    }

    /// <summary>
    /// Tạo Gateway mặc định (COD - phổ biến nhất)
    /// </summary>
    public IPaymentGateway CreateDefaultGateway()
    {
        return CreateGateway("COD");
    }
}

/// <summary>
/// Thông tin cổng thanh toán
/// </summary>
public record PaymentGatewayInfo(
    string Code, 
    string DisplayName, 
    string? LogoUrl, 
    bool RequiresOnlinePayment);


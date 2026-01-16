using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Factory Pattern - Tạo đúng loại Payment Service dựa trên phương thức thanh toán
/// 
/// Áp dụng:
/// - Factory Pattern: Tạo object mà không cần biết class cụ thể
/// - ĐA HÌNH (Polymorphism): Trả về IPaymentService, nhưng thực chất là Momo/COD/...
/// 
/// Lợi ích:
/// - Controller không cần biết chi tiết các class thanh toán
/// - Dễ dàng thêm phương thức thanh toán mới (VNPay, ZaloPay...)
/// - Tập trung logic tạo object vào một chỗ
/// </summary>
public class PaymentFactory
{
    /// <summary>
    /// Tạo Payment Service dựa trên phương thức thanh toán
    /// </summary>
    /// <param name="paymentMethod">Phương thức thanh toán (MOMO, COD, VNPAY...)</param>
    /// <returns>IPaymentService tương ứng</returns>
    /// <exception cref="ArgumentException">Ném ra khi phương thức không hỗ trợ</exception>
    public IPaymentService GetPaymentService(string paymentMethod)
    {
        return paymentMethod.ToUpper() switch
        {
            "MOMO" => new MomoPaymentService(),
            "COD" => new CodPaymentService(),
            // Dễ dàng thêm phương thức mới:
            // "VNPAY" => new VnPayPaymentService(),
            // "ZALOPAY" => new ZaloPayPaymentService(),
            _ => throw new ArgumentException($"Phương thức thanh toán '{paymentMethod}' không được hỗ trợ")
        };
    }

    /// <summary>
    /// Lấy danh sách các phương thức thanh toán được hỗ trợ
    /// </summary>
    public IEnumerable<string> GetSupportedMethods()
    {
        return new[] { "MOMO", "COD" };
    }
}


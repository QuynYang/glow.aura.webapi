namespace CosmeticStore.Core.Enums;

/// <summary>
/// Enum phương thức thanh toán - Dùng trong Factory Pattern
/// </summary>
public enum PaymentMethod
{
    /// <summary>
    /// Thanh toán khi nhận hàng (Cash On Delivery)
    /// </summary>
    COD = 0,

    /// <summary>
    /// Thanh toán qua ví Momo
    /// </summary>
    Momo = 1,

    /// <summary>
    /// Thanh toán qua VNPay
    /// </summary>
    VNPay = 2,

    /// <summary>
    /// Thanh toán qua ZaloPay
    /// </summary>
    ZaloPay = 3,

    /// <summary>
    /// Chuyển khoản ngân hàng
    /// </summary>
    BankTransfer = 4
}

/// <summary>
/// Extension methods cho PaymentMethod
/// </summary>
public static class PaymentMethodExtensions
{
    /// <summary>
    /// Lấy tên hiển thị
    /// </summary>
    public static string GetDisplayName(this PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.COD => "Thanh toán khi nhận hàng",
            PaymentMethod.Momo => "Ví Momo",
            PaymentMethod.VNPay => "VNPay",
            PaymentMethod.ZaloPay => "ZaloPay",
            PaymentMethod.BankTransfer => "Chuyển khoản ngân hàng",
            _ => "Không xác định"
        };
    }

    /// <summary>
    /// Kiểm tra có cần thanh toán online không
    /// </summary>
    public static bool RequiresOnlinePayment(this PaymentMethod method)
    {
        return method is PaymentMethod.Momo or PaymentMethod.VNPay or PaymentMethod.ZaloPay;
    }
}


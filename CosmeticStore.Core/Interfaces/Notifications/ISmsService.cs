namespace CosmeticStore.Core.Interfaces.Notifications;

/// <summary>
/// Abstract Product: Interface cho dịch vụ gửi SMS.
/// Đây là một phần của Abstract Factory Pattern.
/// </summary>
public interface ISmsService
{
    /// <summary>
    /// Gửi tin nhắn SMS đến số điện thoại.
    /// </summary>
    /// <param name="phoneNumber">Số điện thoại người nhận</param>
    /// <param name="message">Nội dung tin nhắn</param>
    /// <returns>True nếu gửi thành công</returns>
    Task<bool> SendSmsAsync(string phoneNumber, string message);

    /// <summary>
    /// Gửi SMS xác nhận đơn hàng.
    /// </summary>
    Task<bool> SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, string orderNumber, decimal totalAmount);

    /// <summary>
    /// Gửi SMS thông báo khuyến mãi.
    /// </summary>
    Task<bool> SendPromotionSmsAsync(string phoneNumber, string customerName, string promotionCode);

    /// <summary>
    /// Gửi SMS chào mừng khách hàng mới.
    /// </summary>
    Task<bool> SendWelcomeSmsAsync(string phoneNumber, string customerName);

    /// <summary>
    /// Gửi SMS thông báo đơn hàng đã giao.
    /// </summary>
    Task<bool> SendOrderDeliveredSmsAsync(string phoneNumber, string customerName, string orderNumber);

    /// <summary>
    /// Lấy kiểu tin nhắn đang sử dụng (để phân biệt Luxury vs Standard).
    /// </summary>
    string MessageStyle { get; }
}


namespace CosmeticStore.Core.Interfaces.Notifications;

/// <summary>
/// Abstract Product: Interface cho dịch vụ gửi Email.
/// Đây là một phần của Abstract Factory Pattern.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Gửi email đến người nhận.
    /// </summary>
    /// <param name="to">Địa chỉ email người nhận</param>
    /// <param name="subject">Tiêu đề email</param>
    /// <param name="body">Nội dung email (có thể là HTML)</param>
    /// <param name="isHtml">Nội dung có phải là HTML không</param>
    /// <returns>True nếu gửi thành công</returns>
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Gửi email xác nhận đơn hàng.
    /// </summary>
    Task<bool> SendOrderConfirmationAsync(string to, string customerName, string orderNumber, decimal totalAmount);

    /// <summary>
    /// Gửi email thông báo khuyến mãi.
    /// </summary>
    Task<bool> SendPromotionAsync(string to, string customerName, string promotionTitle, string promotionDetails);

    /// <summary>
    /// Gửi email chào mừng khách hàng mới.
    /// </summary>
    Task<bool> SendWelcomeEmailAsync(string to, string customerName);

    /// <summary>
    /// Gửi email thông báo đơn hàng đã giao.
    /// </summary>
    Task<bool> SendOrderDeliveredAsync(string to, string customerName, string orderNumber);

    /// <summary>
    /// Lấy tên template đang sử dụng (để phân biệt Luxury vs Standard).
    /// </summary>
    string TemplateName { get; }
}


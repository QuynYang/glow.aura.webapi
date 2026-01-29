namespace CosmeticStore.Core.Interfaces.Notifications;


/// Abstract Product: Interface cho dịch vụ gửi Email.
/// Đây là một phần của Abstract Factory Pattern.

public interface IEmailService
{
    
    /// Gửi email đến người nhận.
    
    /// <param name="to">Địa chỉ email người nhận</param>
    /// <param name="subject">Tiêu đề email</param>
    /// <param name="body">Nội dung email (có thể là HTML)</param>
    /// <param name="isHtml">Nội dung có phải là HTML không</param>
    /// <returns>True nếu gửi thành công</returns>
    Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    
    /// Gửi email xác nhận đơn hàng.
    
    Task<bool> SendOrderConfirmationAsync(string to, string customerName, string orderNumber, decimal totalAmount);

    
    /// Gửi email thông báo khuyến mãi.
    
    Task<bool> SendPromotionAsync(string to, string customerName, string promotionTitle, string promotionDetails);

    
    /// Gửi email chào mừng khách hàng mới.
    
    Task<bool> SendWelcomeEmailAsync(string to, string customerName);

    
    /// Gửi email thông báo đơn hàng đã giao.
    
    Task<bool> SendOrderDeliveredAsync(string to, string customerName, string orderNumber);

    
    /// Lấy tên template đang sử dụng (để phân biệt Luxury vs Standard).
    
    string TemplateName { get; }
}


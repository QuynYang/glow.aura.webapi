namespace CosmeticStore.Core.Interfaces.Notifications;


/// Abstract Product: Interface cho dịch vụ gửi SMS.
/// Đây là một phần của Abstract Factory Pattern.

public interface ISmsService
{
    /// Gửi tin nhắn SMS đến số điện thoại.
    
    /// <param name="phoneNumber">Số điện thoại người nhận</param>
    /// <param name="message">Nội dung tin nhắn</param>
    /// <returns>True nếu gửi thành công</returns>
    Task<bool> SendSmsAsync(string phoneNumber, string message);

    
    /// Gửi SMS xác nhận đơn hàng.
    
    Task<bool> SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, string orderNumber, decimal totalAmount);

    
    /// Gửi SMS thông báo khuyến mãi.
    
    Task<bool> SendPromotionSmsAsync(string phoneNumber, string customerName, string promotionCode);

    
    /// Gửi SMS chào mừng khách hàng mới.
    
    Task<bool> SendWelcomeSmsAsync(string phoneNumber, string customerName);

    
    /// Gửi SMS thông báo đơn hàng đã giao.
    
    Task<bool> SendOrderDeliveredSmsAsync(string phoneNumber, string customerName, string orderNumber);

    
    /// Lấy kiểu tin nhắn đang sử dụng (để phân biệt Luxury vs Standard).
    
    string MessageStyle { get; }
}


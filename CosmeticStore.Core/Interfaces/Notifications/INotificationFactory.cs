namespace CosmeticStore.Core.Interfaces.Notifications;


/// Abstract Factory: Interface tạo ra họ các đối tượng Notification liên quan.
/// Mỗi Factory cụ thể sẽ tạo ra bộ Email + SMS phù hợp với loại khách hàng.
/// 
/// Pattern: Abstract Factory
/// - LuxuryNotificationFactory: Tạo Email sang trọng + SMS cá nhân hóa
/// - StandardNotificationFactory: Tạo Email chuẩn + SMS ngắn gọn

public interface INotificationFactory
{
    
    /// Tạo dịch vụ Email phù hợp với loại Factory.
    
    /// <returns>IEmailService tương ứng (Luxury hoặc Standard)</returns>
    IEmailService CreateEmailService();

    
    /// Tạo dịch vụ SMS phù hợp với loại Factory.
    
    /// <returns>ISmsService tương ứng (Luxury hoặc Standard)</returns>
    ISmsService CreateSmsService();

    
    /// Tên của Factory (để log và debug).
    
    string FactoryName { get; }
}


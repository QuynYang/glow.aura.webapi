namespace CosmeticStore.Core.Interfaces.Notifications;

/// <summary>
/// Abstract Factory: Interface tạo ra họ các đối tượng Notification liên quan.
/// Mỗi Factory cụ thể sẽ tạo ra bộ Email + SMS phù hợp với loại khách hàng.
/// 
/// Pattern: Abstract Factory
/// - LuxuryNotificationFactory: Tạo Email sang trọng + SMS cá nhân hóa
/// - StandardNotificationFactory: Tạo Email chuẩn + SMS ngắn gọn
/// </summary>
public interface INotificationFactory
{
    /// <summary>
    /// Tạo dịch vụ Email phù hợp với loại Factory.
    /// </summary>
    /// <returns>IEmailService tương ứng (Luxury hoặc Standard)</returns>
    IEmailService CreateEmailService();

    /// <summary>
    /// Tạo dịch vụ SMS phù hợp với loại Factory.
    /// </summary>
    /// <returns>ISmsService tương ứng (Luxury hoặc Standard)</returns>
    ISmsService CreateSmsService();

    /// <summary>
    /// Tên của Factory (để log và debug).
    /// </summary>
    string FactoryName { get; }
}


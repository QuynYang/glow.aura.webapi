using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Factory: Tạo họ sản phẩm Notification cho khách hàng thường.
/// - StandardEmailService: Email template chuẩn, chuyên nghiệp
/// - StandardSmsService: SMS ngắn gọn, tự động
/// 
/// Pattern: Abstract Factory - Factory này tạo ra "family" các đối tượng
/// liên quan đến nhau (Email + SMS standard) mà client không cần biết chi tiết.
/// </summary>
public class StandardNotificationFactory : INotificationFactory
{
    private readonly ISystemLogger _logger;

    public string FactoryName => "Standard Notification Factory";

    public StandardNotificationFactory(ISystemLogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tạo Email Service với template chuẩn cho khách hàng thường.
    /// </summary>
    public IEmailService CreateEmailService()
    {
        _logger.LogDebug($"[{FactoryName}] Creating StandardEmailService");
        return new StandardEmailService(_logger);
    }

    /// <summary>
    /// Tạo SMS Service với tin nhắn ngắn gọn cho khách hàng thường.
    /// </summary>
    public ISmsService CreateSmsService()
    {
        _logger.LogDebug($"[{FactoryName}] Creating StandardSmsService");
        return new StandardSmsService(_logger);
    }
}


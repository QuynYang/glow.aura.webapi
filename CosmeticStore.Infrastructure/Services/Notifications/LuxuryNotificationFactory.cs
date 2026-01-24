using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Factory: Tạo họ sản phẩm Notification cho khách hàng VIP.
/// - LuxuryEmailService: Email template vàng, sang trọng
/// - LuxurySmsService: SMS phong cách trợ lý cá nhân
/// 
/// Pattern: Abstract Factory - Factory này tạo ra "family" các đối tượng
/// liên quan đến nhau (Email + SMS luxury) mà client không cần biết chi tiết.
/// </summary>
public class LuxuryNotificationFactory : INotificationFactory
{
    private readonly ISystemLogger _logger;

    public string FactoryName => "Luxury Notification Factory (VIP)";

    public LuxuryNotificationFactory(ISystemLogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Tạo Email Service với template sang trọng cho khách VIP.
    /// </summary>
    public IEmailService CreateEmailService()
    {
        _logger.LogDebug($"[{FactoryName}] Creating LuxuryEmailService");
        return new LuxuryEmailService(_logger);
    }

    /// <summary>
    /// Tạo SMS Service với phong cách trợ lý cá nhân cho khách VIP.
    /// </summary>
    public ISmsService CreateSmsService()
    {
        _logger.LogDebug($"[{FactoryName}] Creating LuxurySmsService");
        return new LuxurySmsService(_logger);
    }
}


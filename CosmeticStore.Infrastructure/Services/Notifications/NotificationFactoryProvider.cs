using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Provider để lựa chọn NotificationFactory phù hợp dựa trên VIP Level.
/// Đây là điểm entry để sử dụng Abstract Factory Pattern trong ứng dụng.
/// 
/// Logic phân chia:
/// - Gold, Platinum, Diamond → LuxuryNotificationFactory
/// - None, Silver → StandardNotificationFactory
/// </summary>
public class NotificationFactoryProvider : INotificationFactoryProvider
{
    private readonly ISystemLogger _logger;
    private readonly LuxuryNotificationFactory _luxuryFactory;
    private readonly StandardNotificationFactory _standardFactory;

    public NotificationFactoryProvider(ISystemLogger logger)
    {
        _logger = logger;
        _luxuryFactory = new LuxuryNotificationFactory(logger);
        _standardFactory = new StandardNotificationFactory(logger);
    }

    /// <summary>
    /// Lấy Factory phù hợp dựa trên VIP Level của khách hàng.
    /// </summary>
    public INotificationFactory GetFactory(VipLevel vipLevel)
    {
        INotificationFactory factory = vipLevel switch
        {
            VipLevel.Gold => _luxuryFactory,
            VipLevel.Platinum => _luxuryFactory,
            _ => _standardFactory
        };

        _logger.LogDebug($"Selected factory for VipLevel.{vipLevel}: {factory.FactoryName}");
        
        return factory;
    }

    /// <summary>
    /// Lấy Factory mặc định (Standard) cho khách hàng thường.
    /// </summary>
    public INotificationFactory GetDefaultFactory()
    {
        return _standardFactory;
    }

    /// <summary>
    /// Lấy Factory Luxury cho khách hàng VIP.
    /// </summary>
    public INotificationFactory GetLuxuryFactory()
    {
        return _luxuryFactory;
    }
}


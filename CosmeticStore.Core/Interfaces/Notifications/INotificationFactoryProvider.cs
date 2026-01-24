using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces.Notifications;

/// <summary>
/// Provider để lựa chọn NotificationFactory phù hợp dựa trên VIP Level của khách hàng.
/// Đây là điểm entry để sử dụng Abstract Factory Pattern trong ứng dụng.
/// </summary>
public interface INotificationFactoryProvider
{
    /// <summary>
    /// Lấy Factory phù hợp dựa trên VIP Level.
    /// - Gold, Platinum, Diamond -> LuxuryNotificationFactory
    /// - None, Silver -> StandardNotificationFactory
    /// </summary>
    /// <param name="vipLevel">Cấp VIP của khách hàng</param>
    /// <returns>INotificationFactory tương ứng</returns>
    INotificationFactory GetFactory(VipLevel vipLevel);

    /// <summary>
    /// Lấy Factory mặc định (Standard).
    /// </summary>
    INotificationFactory GetDefaultFactory();

    /// <summary>
    /// Lấy Factory Luxury.
    /// </summary>
    INotificationFactory GetLuxuryFactory();
}


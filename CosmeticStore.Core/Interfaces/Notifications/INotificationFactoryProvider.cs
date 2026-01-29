using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces.Notifications;


/// Provider để lựa chọn NotificationFactory phù hợp dựa trên VIP Level của khách hàng.
/// Đây là điểm entry để sử dụng Abstract Factory Pattern trong ứng dụng.

public interface INotificationFactoryProvider
{
    
    /// Lấy Factory phù hợp dựa trên VIP Level.
    /// - Gold, Platinum, Diamond -> LuxuryNotificationFactory
    /// - None, Silver -> StandardNotificationFactory
    
    /// <param name="vipLevel">Cấp VIP của khách hàng</param>
    /// <returns>INotificationFactory tương ứng</returns>
    INotificationFactory GetFactory(VipLevel vipLevel);

    
    /// Lấy Factory mặc định (Standard).
    
    INotificationFactory GetDefaultFactory();

    
    /// Lấy Factory Luxury.
    
    INotificationFactory GetLuxuryFactory();
}


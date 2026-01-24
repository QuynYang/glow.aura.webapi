using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Product: SMS Service cho khách hàng VIP (Gold/Platinum/Diamond).
/// Tin nhắn phong cách "Trợ lý cá nhân", ngôn ngữ trang trọng.
/// </summary>
public class LuxurySmsService : ISmsService
{
    private readonly ISystemLogger _logger;

    public string MessageStyle => "Personal Assistant Style";

    public LuxurySmsService(ISystemLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        // Format tin nhắn theo phong cách trợ lý cá nhân
        var formattedMessage = $"[GlowAura VIP] {message}";

        _logger.LogInfo($"[LUXURY SMS] Sending to: {phoneNumber}", new
        {
            Message = formattedMessage,
            Style = MessageStyle
        });

        // TODO: Tích hợp với SMS Provider thực tế (Twilio, Vonage, etc.)
        Console.WriteLine($"📱 [LUXURY SMS] To {phoneNumber}: {formattedMessage}");
        
        return await Task.FromResult(true);
    }

    public async Task<bool> SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, string orderNumber, decimal totalAmount)
    {
        var message = $"Kính gửi Quý khách {customerName}, " +
                      $"Đơn hàng #{orderNumber} trị giá {totalAmount:N0}đ đã được tiếp nhận. " +
                      $"Chuyên viên chăm sóc VIP sẽ liên hệ Quý khách trong 30 phút. " +
                      $"Hotline VIP: 1900-GLOW 💎";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendPromotionSmsAsync(string phoneNumber, string customerName, string promotionCode)
    {
        var message = $"Kính gửi Quý khách VIP {customerName}, " +
                      $"GlowAura trân trọng gửi tặng ưu đãi ĐỘC QUYỀN. " +
                      $"Mã: {promotionCode}. " +
                      $"Liên hệ trợ lý cá nhân để được tư vấn: 1900-GLOW-VIP 👑";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendWelcomeSmsAsync(string phoneNumber, string customerName)
    {
        var message = $"Kính chào Quý khách {customerName}, " +
                      $"Chào mừng đến với GlowAura VIP Club! " +
                      $"Quý khách được hưởng ưu đãi giảm 30% và giao hàng ưu tiên miễn phí. " +
                      $"Hotline VIP 24/7: 1900-GLOW-VIP 💎";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendOrderDeliveredSmsAsync(string phoneNumber, string customerName, string orderNumber)
    {
        var message = $"Kính gửi Quý khách {customerName}, " +
                      $"Đơn hàng #{orderNumber} đã giao thành công. " +
                      $"Cảm ơn Quý khách đã tin tưởng GlowAura. " +
                      $"Đánh giá sản phẩm để nhận thêm điểm VIP! 🌟";
        
        return await SendSmsAsync(phoneNumber, message);
    }
}


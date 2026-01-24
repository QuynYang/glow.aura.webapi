using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Product: SMS Service cho khách hàng thường (None/Silver).
/// Tin nhắn ngắn gọn, tự động, tập trung vào thông tin chính.
/// </summary>
public class StandardSmsService : ISmsService
{
    private readonly ISystemLogger _logger;

    public string MessageStyle => "Standard Auto Style";

    public StandardSmsService(ISystemLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        // Format tin nhắn ngắn gọn
        var formattedMessage = $"[GlowAura] {message}";

        _logger.LogInfo($"[STANDARD SMS] Sending to: {phoneNumber}", new
        {
            Message = formattedMessage,
            Style = MessageStyle
        });

        // TODO: Tích hợp với SMS Provider thực tế
        Console.WriteLine($"📱 [STANDARD SMS] To {phoneNumber}: {formattedMessage}");
        
        return await Task.FromResult(true);
    }

    public async Task<bool> SendOrderConfirmationSmsAsync(string phoneNumber, string customerName, string orderNumber, decimal totalAmount)
    {
        var message = $"Don hang #{orderNumber} da duoc xac nhan. " +
                      $"Tong: {totalAmount:N0}d. " +
                      $"GlowAura cam on ban!";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendPromotionSmsAsync(string phoneNumber, string customerName, string promotionCode)
    {
        var message = $"GlowAura: Ma giam gia {promotionCode}. " +
                      $"Ap dung ngay tai glowaura.vn. " +
                      $"HSD: 7 ngay.";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendWelcomeSmsAsync(string phoneNumber, string customerName)
    {
        var message = $"Chao mung {customerName} den GlowAura! " +
                      $"Dang ky thanh cong. " +
                      $"Mua sam ngay: glowaura.vn";
        
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendOrderDeliveredSmsAsync(string phoneNumber, string customerName, string orderNumber)
    {
        var message = $"Don hang #{orderNumber} da giao thanh cong. " +
                      $"Cam on ban da mua hang tai GlowAura!";
        
        return await SendSmsAsync(phoneNumber, message);
    }
}


using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Product: Email Service cho khách hàng thường (None/Silver).
/// Template đơn giản, chuyên nghiệp, tập trung vào thông tin.
/// </summary>
public class StandardEmailService : IEmailService
{
    private readonly ISystemLogger _logger;

    public string TemplateName => "Standard Professional Template";

    public StandardEmailService(ISystemLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        var standardBody = WrapWithStandardTemplate(subject, body);

        _logger.LogInfo($"[STANDARD EMAIL] Sending to: {to}", new
        {
            Subject = subject,
            Template = TemplateName,
            IsHtml = isHtml
        });

        // TODO: Tích hợp với Email Provider thực tế
        Console.WriteLine($"📧 [STANDARD] Email sent to {to}: {subject}");
        
        return await Task.FromResult(true);
    }

    public async Task<bool> SendOrderConfirmationAsync(string to, string customerName, string orderNumber, decimal totalAmount)
    {
        var subject = $"Xác nhận đơn hàng #{orderNumber} - GlowAura";
        var body = $@"
            <h2>Xác nhận đơn hàng</h2>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Cảm ơn bạn đã đặt hàng tại GlowAura!</p>
            <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
                <tr style='background: #f5f5f5;'>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Mã đơn hàng</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>#{orderNumber}</td>
                </tr>
                <tr>
                    <td style='padding: 10px; border: 1px solid #ddd;'><strong>Tổng tiền</strong></td>
                    <td style='padding: 10px; border: 1px solid #ddd;'>{totalAmount:N0} VNĐ</td>
                </tr>
            </table>
            <p>Chúng tôi sẽ liên hệ bạn để xác nhận đơn hàng.</p>
            <p>Trân trọng,<br/>Đội ngũ GlowAura</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendPromotionAsync(string to, string customerName, string promotionTitle, string promotionDetails)
    {
        var subject = $"Ưu đãi đặc biệt từ GlowAura - {promotionTitle}";
        var body = $@"
            <h2>{promotionTitle}</h2>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>GlowAura có chương trình khuyến mãi đặc biệt dành cho bạn:</p>
            <div style='background: #f5f5f5; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                <p>{promotionDetails}</p>
            </div>
            <p>Đừng bỏ lỡ cơ hội này!</p>
            <p>Trân trọng,<br/>Đội ngũ GlowAura</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendWelcomeEmailAsync(string to, string customerName)
    {
        var subject = "Chào mừng bạn đến với GlowAura!";
        var body = $@"
            <h2>Chào mừng đến với GlowAura!</h2>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Cảm ơn bạn đã đăng ký tài khoản tại GlowAura - Hệ thống mỹ phẩm chính hãng.</p>
            <div style='background: #f5f5f5; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                <h3>Bạn có thể:</h3>
                <ul>
                    <li>Mua sắm với giá ưu đãi</li>
                    <li>Tích điểm với mỗi đơn hàng</li>
                    <li>Nhận thông báo khuyến mãi</li>
                    <li>Nâng cấp VIP để nhận thêm ưu đãi</li>
                </ul>
            </div>
            <p>Chúc bạn có trải nghiệm mua sắm tuyệt vời!</p>
            <p>Trân trọng,<br/>Đội ngũ GlowAura</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendOrderDeliveredAsync(string to, string customerName, string orderNumber)
    {
        var subject = $"Đơn hàng #{orderNumber} đã được giao";
        var body = $@"
            <h2>Giao hàng thành công!</h2>
            <p>Xin chào <strong>{customerName}</strong>,</p>
            <p>Đơn hàng <strong>#{orderNumber}</strong> của bạn đã được giao thành công.</p>
            <p>Cảm ơn bạn đã mua sắm tại GlowAura!</p>
            <div style='background: #f5f5f5; padding: 15px; border-radius: 5px; margin: 15px 0;'>
                <p>Bạn có hài lòng với sản phẩm? Hãy để lại đánh giá để nhận điểm thưởng!</p>
            </div>
            <p>Trân trọng,<br/>Đội ngũ GlowAura</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    private string WrapWithStandardTemplate(string title, string content)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; background: #f5f5f5; color: #333; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 20px auto; background: #fff; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        .header {{ background: #FF6B9D; padding: 20px; text-align: center; border-radius: 8px 8px 0 0; }}
        .header h1 {{ color: #fff; margin: 0; font-size: 24px; }}
        .content {{ padding: 30px; line-height: 1.6; }}
        .footer {{ background: #f5f5f5; padding: 20px; text-align: center; border-radius: 0 0 8px 8px; color: #666; font-size: 14px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>GlowAura</h1>
        </div>
        <div class='content'>
            {content}
        </div>
        <div class='footer'>
            <p>GlowAura - Mỹ phẩm chính hãng</p>
            <p>Hotline: 1900-GLOW | Email: support@glowaura.vn</p>
        </div>
    </div>
</body>
</html>";
    }
}


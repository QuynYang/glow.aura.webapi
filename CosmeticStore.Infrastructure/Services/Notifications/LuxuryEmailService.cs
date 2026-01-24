using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Services.Notifications;

/// <summary>
/// Concrete Product: Email Service cho khách hàng VIP (Gold/Platinum/Diamond).
/// Sử dụng template sang trọng với giao diện Gold, nội dung cá nhân hóa.
/// </summary>
public class LuxuryEmailService : IEmailService
{
    private readonly ISystemLogger _logger;

    public string TemplateName => "Luxury Gold Template";

    public LuxuryEmailService(ISystemLogger logger)
    {
        _logger = logger;
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        // Template email sang trọng với header vàng và thiết kế cao cấp
        var luxuryBody = WrapWithLuxuryTemplate(subject, body);

        _logger.LogInfo($"[LUXURY EMAIL] Sending to: {to}", new
        {
            Subject = subject,
            Template = TemplateName,
            IsHtml = isHtml
        });

        // TODO: Tích hợp với Email Provider thực tế (SendGrid, Mailgun, etc.)
        // Hiện tại chỉ log để demo
        Console.WriteLine($"📧 [LUXURY] Email sent to {to}: {subject}");
        
        return await Task.FromResult(true);
    }

    public async Task<bool> SendOrderConfirmationAsync(string to, string customerName, string orderNumber, decimal totalAmount)
    {
        var subject = $"✨ Cảm ơn Quý khách {customerName} - Đơn hàng #{orderNumber}";
        var body = $@"
            <div style='text-align: center; margin-bottom: 20px;'>
                <h1 style='color: #D4AF37;'>🌟 XÁC NHẬN ĐƠN HÀNG 🌟</h1>
            </div>
            <p>Kính gửi <strong style='color: #D4AF37;'>{customerName}</strong>,</p>
            <p>Chúng tôi vô cùng vinh hạnh được phục vụ Quý khách!</p>
            <div style='background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 100%); padding: 20px; border-radius: 10px; margin: 20px 0;'>
                <p style='color: #1a1a1a; font-size: 18px;'>
                    <strong>Mã đơn hàng:</strong> #{orderNumber}<br/>
                    <strong>Tổng giá trị:</strong> {totalAmount:N0} VNĐ
                </p>
            </div>
            <p>Đội ngũ chăm sóc khách hàng VIP sẽ liên hệ Quý khách trong vòng 30 phút.</p>
            <p style='color: #D4AF37;'>💎 Tri ân khách hàng thân thiết 💎</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendPromotionAsync(string to, string customerName, string promotionTitle, string promotionDetails)
    {
        var subject = $"👑 Ưu đãi độc quyền dành riêng cho {customerName}";
        var body = $@"
            <div style='text-align: center;'>
                <h1 style='color: #D4AF37;'>👑 ƯU ĐÃI VIP ĐỘC QUYỀN 👑</h1>
            </div>
            <p>Kính gửi <strong style='color: #D4AF37;'>{customerName}</strong>,</p>
            <p>Với tư cách là thành viên VIP, Quý khách được hưởng ưu đãi đặc biệt:</p>
            <div style='background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 100%); padding: 20px; border-radius: 10px; margin: 20px 0;'>
                <h2 style='color: #1a1a1a; text-align: center;'>{promotionTitle}</h2>
                <p style='color: #1a1a1a;'>{promotionDetails}</p>
            </div>
            <p>Ưu đãi này chỉ dành riêng cho khách hàng VIP như Quý khách.</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendWelcomeEmailAsync(string to, string customerName)
    {
        var subject = $"✨ Chào mừng {customerName} gia nhập CLB Khách hàng VIP";
        var body = $@"
            <div style='text-align: center;'>
                <h1 style='color: #D4AF37;'>🌟 CHÀO MỪNG THÀNH VIÊN VIP 🌟</h1>
            </div>
            <p>Kính chào <strong style='color: #D4AF37;'>{customerName}</strong>,</p>
            <p>Chúng tôi vô cùng vinh dự chào đón Quý khách trở thành thành viên VIP của GlowAura!</p>
            <div style='background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 100%); padding: 20px; border-radius: 10px; margin: 20px 0;'>
                <h3 style='color: #1a1a1a;'>Đặc quyền VIP của Quý khách:</h3>
                <ul style='color: #1a1a1a;'>
                    <li>🎁 Giảm giá độc quyền lên đến 30%</li>
                    <li>📦 Giao hàng ưu tiên miễn phí</li>
                    <li>💆 Tư vấn làm đẹp riêng 1-1</li>
                    <li>🎉 Quà tặng sinh nhật đặc biệt</li>
                    <li>📞 Hotline VIP 24/7</li>
                </ul>
            </div>
            <p style='color: #D4AF37;'>💎 Trải nghiệm đẳng cấp cùng GlowAura 💎</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    public async Task<bool> SendOrderDeliveredAsync(string to, string customerName, string orderNumber)
    {
        var subject = $"🎉 Đơn hàng #{orderNumber} đã được giao thành công";
        var body = $@"
            <div style='text-align: center;'>
                <h1 style='color: #D4AF37;'>✅ GIAO HÀNG THÀNH CÔNG ✅</h1>
            </div>
            <p>Kính gửi <strong style='color: #D4AF37;'>{customerName}</strong>,</p>
            <p>Đơn hàng <strong>#{orderNumber}</strong> của Quý khách đã được giao thành công!</p>
            <p>Cảm ơn Quý khách đã tin tưởng GlowAura. Chúng tôi luôn nỗ lực mang đến trải nghiệm tốt nhất.</p>
            <div style='background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 100%); padding: 15px; border-radius: 10px; margin: 20px 0; text-align: center;'>
                <p style='color: #1a1a1a;'>Quý khách hài lòng? Hãy để lại đánh giá và nhận thêm điểm thưởng VIP!</p>
            </div>
            <p style='color: #D4AF37;'>💎 Hẹn gặp lại Quý khách 💎</p>
        ";
        
        return await SendEmailAsync(to, subject, body);
    }

    private string WrapWithLuxuryTemplate(string title, string content)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: 'Georgia', serif; background: #1a1a1a; color: #fff; margin: 0; padding: 0; }}
        .container {{ max-width: 600px; margin: 0 auto; background: linear-gradient(180deg, #1a1a1a 0%, #2d2d2d 100%); }}
        .header {{ background: linear-gradient(135deg, #D4AF37 0%, #F5E6A3 50%, #D4AF37 100%); padding: 30px; text-align: center; }}
        .header h1 {{ color: #1a1a1a; margin: 0; font-size: 28px; text-transform: uppercase; letter-spacing: 3px; }}
        .content {{ padding: 30px; line-height: 1.8; }}
        .footer {{ background: #D4AF37; padding: 20px; text-align: center; color: #1a1a1a; }}
        .footer p {{ margin: 5px 0; }}
        .vip-badge {{ display: inline-block; background: #D4AF37; color: #1a1a1a; padding: 5px 15px; border-radius: 20px; font-weight: bold; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <span class='vip-badge'>👑 VIP MEMBER</span>
            <h1>GlowAura Luxury</h1>
        </div>
        <div class='content'>
            {content}
        </div>
        <div class='footer'>
            <p><strong>GlowAura - Luxury Cosmetics</strong></p>
            <p>Hotline VIP: 1900-GLOW-VIP</p>
            <p>💎 Đẳng cấp làm đẹp 💎</p>
        </div>
    </div>
</body>
</html>";
    }
}


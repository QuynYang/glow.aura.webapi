using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.Interfaces.Notifications;

namespace CosmeticStore.Infrastructure.Handlers.Notifications;

/// <summary>
/// VIP-Aware Notification Handler - Sử dụng ABSTRACT FACTORY PATTERN
/// 
/// Handler này chọn Factory phù hợp dựa trên VIP Level của khách hàng:
/// - VIP (Gold/Platinum/Diamond): Dùng LuxuryNotificationFactory
///   → Email template sang trọng + SMS cá nhân hóa
/// - Standard (None/Silver): Dùng StandardNotificationFactory  
///   → Email template chuẩn + SMS ngắn gọn
/// 
/// ABSTRACT FACTORY PATTERN cho phép:
/// - Tạo "họ" đối tượng liên quan (Email + SMS) mà không cần chỉ định class cụ thể
/// - Client code không phụ thuộc vào implementation cụ thể
/// - Dễ dàng thêm "họ" mới (ví dụ: PremiumNotificationFactory)
/// </summary>
public class VipAwareOrderCreatedHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly INotificationFactoryProvider _factoryProvider;
    private readonly ISystemLogger _logger;

    public string HandlerName => "VipAwareOrderCreatedHandler";
    public int Priority => 5; // Ưu tiên cao nhất

    public VipAwareOrderCreatedHandler(
        INotificationFactoryProvider factoryProvider,
        ISystemLogger logger)
    {
        _factoryProvider = factoryProvider;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // 1. Lấy VIP Level từ event (mặc định là None nếu không có)
        var vipLevel = domainEvent.UserVipLevel;

        // 2. Abstract Factory: Chọn factory phù hợp với VIP level
        var factory = _factoryProvider.GetFactory(vipLevel);
        
        _logger.LogInfo($"Using {factory.FactoryName} for VipLevel.{vipLevel}", new
        {
            domainEvent.OrderId,
            domainEvent.OrderNumber,
            VipLevel = vipLevel.ToString()
        });

        // 3. Factory tạo ra Email Service phù hợp (Luxury hoặc Standard)
        var emailService = factory.CreateEmailService();
        
        // 4. Gửi email xác nhận đơn hàng (template tự động theo loại factory)
        await emailService.SendOrderConfirmationAsync(
            domainEvent.UserEmail,
            domainEvent.UserName,
            domainEvent.OrderNumber,
            domainEvent.TotalAmount
        );

        // 5. Factory tạo ra SMS Service phù hợp
        if (!string.IsNullOrEmpty(domainEvent.UserPhone))
        {
            var smsService = factory.CreateSmsService();
            
            await smsService.SendOrderConfirmationSmsAsync(
                domainEvent.UserPhone,
                domainEvent.UserName,
                domainEvent.OrderNumber,
                domainEvent.TotalAmount
            );
        }

        _logger.LogInfo($"VIP-aware notifications sent", new
        {
            domainEvent.OrderId,
            domainEvent.OrderNumber,
            EmailTemplate = emailService.TemplateName,
            VipLevel = vipLevel.ToString()
        });
    }
}

/// <summary>
/// Handler gửi thông báo welcome theo VIP level - ABSTRACT FACTORY PATTERN
/// </summary>
public class VipAwareWelcomeHandler : IDomainEventHandler<UserRegisteredEvent>
{
    private readonly INotificationFactoryProvider _factoryProvider;
    private readonly ISystemLogger _logger;

    public string HandlerName => "VipAwareWelcomeHandler";
    public int Priority => 10;

    public VipAwareWelcomeHandler(
        INotificationFactoryProvider factoryProvider,
        ISystemLogger logger)
    {
        _factoryProvider = factoryProvider;
        _logger = logger;
    }

    public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Người dùng mới luôn là Standard
        var factory = _factoryProvider.GetDefaultFactory();
        
        var emailService = factory.CreateEmailService();
        await emailService.SendWelcomeEmailAsync(domainEvent.Email, domainEvent.FullName);

        if (!string.IsNullOrEmpty(domainEvent.PhoneNumber))
        {
            var smsService = factory.CreateSmsService();
            await smsService.SendWelcomeSmsAsync(domainEvent.PhoneNumber, domainEvent.FullName);
        }

        _logger.LogInfo($"Welcome notifications sent to new user", new
        {
            domainEvent.UserId,
            domainEvent.Email,
            Template = emailService.TemplateName
        });
    }
}

/// <summary>
/// Handler gửi thông báo khi VIP level thay đổi - ABSTRACT FACTORY PATTERN
/// Khi user được nâng cấp VIP, gửi email chào mừng với template luxury
/// </summary>
public class VipLevelUpgradedHandler : IDomainEventHandler<VipLevelUpgradedEvent>
{
    private readonly INotificationFactoryProvider _factoryProvider;
    private readonly ISystemLogger _logger;

    public string HandlerName => "VipLevelUpgradedHandler";
    public int Priority => 5;

    public VipLevelUpgradedHandler(
        INotificationFactoryProvider factoryProvider,
        ISystemLogger logger)
    {
        _factoryProvider = factoryProvider;
        _logger = logger;
    }

    public async Task HandleAsync(VipLevelUpgradedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Nếu nâng cấp lên Gold+, dùng Luxury factory
        var factory = _factoryProvider.GetFactory(domainEvent.NewVipLevel);
        
        var emailService = factory.CreateEmailService();
        
        // Gửi email chào mừng VIP mới
        var subject = $"🎉 Chúc mừng! Bạn đã trở thành thành viên {domainEvent.NewVipLevel}";
        var body = BuildVipUpgradeEmailBody(domainEvent);
        
        await emailService.SendEmailAsync(domainEvent.Email, subject, body);

        if (!string.IsNullOrEmpty(domainEvent.PhoneNumber))
        {
            var smsService = factory.CreateSmsService();
            await smsService.SendWelcomeSmsAsync(domainEvent.PhoneNumber, domainEvent.FullName);
        }

        _logger.LogInfo($"VIP upgrade notifications sent", new
        {
            domainEvent.UserId,
            OldLevel = domainEvent.OldVipLevel.ToString(),
            NewLevel = domainEvent.NewVipLevel.ToString(),
            Template = emailService.TemplateName
        });
    }

    private static string BuildVipUpgradeEmailBody(VipLevelUpgradedEvent e)
    {
        var benefits = e.NewVipLevel switch
        {
            VipLevel.Bronze => @"
                <li>🎁 Giảm giá 5% cho tất cả sản phẩm</li>
                <li>📦 Ưu tiên xử lý đơn hàng</li>
                <li>🎉 Quà sinh nhật đặc biệt</li>",
            VipLevel.Silver => @"
                <li>🎁 Giảm giá 10% cho tất cả sản phẩm</li>
                <li>📦 Giao hàng ưu tiên</li>
                <li>💆 Tư vấn làm đẹp 1-1</li>
                <li>🎉 Quà sinh nhật đặc biệt</li>",
            VipLevel.Gold => @"
                <li>🎁 Giảm giá 15% cho tất cả sản phẩm</li>
                <li>📦 Giao hàng ưu tiên miễn phí</li>
                <li>💆 Chuyên gia tư vấn riêng</li>
                <li>🎉 Quà sinh nhật Premium</li>
                <li>👑 Truy cập sản phẩm độc quyền</li>",
            VipLevel.Platinum => @"
                <li>🎁 Giảm giá 20% cho tất cả sản phẩm</li>
                <li>📦 Giao hàng Express miễn phí</li>
                <li>💆 Trợ lý cá nhân 24/7</li>
                <li>🎉 Quà sinh nhật Luxury</li>
                <li>👑 Truy cập sản phẩm độc quyền</li>
                <li>✨ Mời tham dự sự kiện VIP</li>",
            _ => "<li>Ưu đãi đặc biệt dành cho bạn</li>"
        };

        return $@"
<p>Xin chúc mừng <strong>{e.FullName}</strong>!</p>
<p>Bạn đã được nâng cấp từ <strong>{e.OldVipLevel}</strong> lên <strong>{e.NewVipLevel}</strong>!</p>
<div style='background: #f5f5f5; padding: 20px; border-radius: 10px; margin: 20px 0;'>
    <h3>Đặc quyền mới của bạn:</h3>
    <ul>{benefits}</ul>
</div>
<p>Cảm ơn bạn đã đồng hành cùng GlowAura!</p>";
    }
}

/// <summary>
/// Handler gửi thông báo khuyến mãi theo VIP level - ABSTRACT FACTORY PATTERN
/// </summary>
public class VipAwarePromotionHandler : IDomainEventHandler<PromotionCreatedEvent>
{
    private readonly INotificationFactoryProvider _factoryProvider;
    private readonly ISystemLogger _logger;

    public string HandlerName => "VipAwarePromotionHandler";
    public int Priority => 30; // Lower priority

    public VipAwarePromotionHandler(
        INotificationFactoryProvider factoryProvider,
        ISystemLogger logger)
    {
        _factoryProvider = factoryProvider;
        _logger = logger;
    }

    public async Task HandleAsync(PromotionCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Phân loại theo target audience
        var factory = domainEvent.IsVipOnly 
            ? _factoryProvider.GetLuxuryFactory() 
            : _factoryProvider.GetDefaultFactory();

        var emailService = factory.CreateEmailService();
        
        // Gửi cho danh sách email nhận khuyến mãi
        foreach (var recipient in domainEvent.Recipients)
        {
            await emailService.SendPromotionAsync(
                recipient.Email,
                recipient.Name,
                domainEvent.PromotionTitle,
                domainEvent.PromotionDetails
            );
        }

        _logger.LogInfo($"Promotion notifications sent", new
        {
            domainEvent.PromotionId,
            domainEvent.PromotionTitle,
            RecipientCount = domainEvent.Recipients.Count,
            IsVipOnly = domainEvent.IsVipOnly
        });
    }
}


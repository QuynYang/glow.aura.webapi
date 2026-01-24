using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Events;

/// <summary>
/// Event khi user đăng ký tài khoản mới
/// 
/// OBSERVER PATTERN + ABSTRACT FACTORY:
/// - Raised by: AuthService khi đăng ký thành công
/// - Handled by: VipAwareWelcomeHandler (gửi email/SMS chào mừng)
/// </summary>
public class UserRegisteredEvent : DomainEventBase
{
    public int UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public string? PhoneNumber { get; }
    public DateTime RegisteredAt { get; }

    public UserRegisteredEvent(
        int userId,
        string email,
        string fullName,
        string? phoneNumber = null)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        PhoneNumber = phoneNumber;
        RegisteredAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Event khi VIP Level của user được nâng cấp
/// 
/// ABSTRACT FACTORY PATTERN:
/// - Handler sử dụng NewVipLevel để chọn NotificationFactory phù hợp
/// - Gold/Platinum/Diamond → LuxuryNotificationFactory
/// </summary>
public class VipLevelUpgradedEvent : DomainEventBase
{
    public int UserId { get; }
    public string Email { get; }
    public string FullName { get; }
    public string? PhoneNumber { get; }
    public VipLevel OldVipLevel { get; }
    public VipLevel NewVipLevel { get; }
    public decimal TotalSpent { get; }

    public VipLevelUpgradedEvent(
        int userId,
        string email,
        string fullName,
        VipLevel oldVipLevel,
        VipLevel newVipLevel,
        decimal totalSpent,
        string? phoneNumber = null)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
        OldVipLevel = oldVipLevel;
        NewVipLevel = newVipLevel;
        TotalSpent = totalSpent;
        PhoneNumber = phoneNumber;
    }
}

/// <summary>
/// Event khi có khuyến mãi mới được tạo
/// 
/// ABSTRACT FACTORY PATTERN:
/// - IsVipOnly = true → Dùng LuxuryNotificationFactory
/// - IsVipOnly = false → Dùng StandardNotificationFactory
/// </summary>
public class PromotionCreatedEvent : DomainEventBase
{
    public int PromotionId { get; }
    public string PromotionTitle { get; }
    public string PromotionDetails { get; }
    public string PromotionCode { get; }
    public decimal DiscountPercent { get; }
    public DateTime StartDate { get; }
    public DateTime EndDate { get; }
    
    /// <summary>
    /// Nếu true, chỉ gửi cho VIP member với Luxury template
    /// </summary>
    public bool IsVipOnly { get; }
    
    /// <summary>
    /// Danh sách người nhận khuyến mãi
    /// </summary>
    public List<PromotionRecipient> Recipients { get; }

    public PromotionCreatedEvent(
        int promotionId,
        string promotionTitle,
        string promotionDetails,
        string promotionCode,
        decimal discountPercent,
        DateTime startDate,
        DateTime endDate,
        bool isVipOnly,
        List<PromotionRecipient>? recipients = null)
    {
        PromotionId = promotionId;
        PromotionTitle = promotionTitle;
        PromotionDetails = promotionDetails;
        PromotionCode = promotionCode;
        DiscountPercent = discountPercent;
        StartDate = startDate;
        EndDate = endDate;
        IsVipOnly = isVipOnly;
        Recipients = recipients ?? new List<PromotionRecipient>();
    }
}

/// <summary>
/// Thông tin người nhận khuyến mãi
/// </summary>
public class PromotionRecipient
{
    public int UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public VipLevel VipLevel { get; set; }
}

/// <summary>
/// Event khi user đăng nhập
/// </summary>
public class UserLoggedInEvent : DomainEventBase
{
    public int UserId { get; }
    public string Email { get; }
    public string? IpAddress { get; }
    public DateTime LoggedInAt { get; }

    public UserLoggedInEvent(int userId, string email, string? ipAddress = null)
    {
        UserId = userId;
        Email = email;
        IpAddress = ipAddress;
        LoggedInAt = DateTime.UtcNow;
    }
}

/// <summary>
/// Event khi user đổi mật khẩu
/// </summary>
public class PasswordChangedEvent : DomainEventBase
{
    public int UserId { get; }
    public string Email { get; }
    public DateTime ChangedAt { get; }

    public PasswordChangedEvent(int userId, string email)
    {
        UserId = userId;
        Email = email;
        ChangedAt = DateTime.UtcNow;
    }
}


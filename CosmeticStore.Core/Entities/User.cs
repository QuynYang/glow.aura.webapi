using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Entities;

/// <summary>
/// Entity đại diện cho người dùng/khách hàng.
/// Áp dụng tính ĐÓNG GÓI (Encapsulation) - Bảo vệ dữ liệu.
/// 
/// Thuộc tính quan trọng cho Strategy Pattern:
/// - VipLevel: Xác định mức giảm giá VIP
/// - SkinType: Xác định loại da để gợi ý sản phẩm và giảm giá
/// </summary>
public class User : BaseEntity
{
    #region Basic Properties

    /// <summary>
    /// Email đăng nhập (unique)
    /// </summary>
    public string Email { get; private set; } = string.Empty;

    /// <summary>
    /// Mật khẩu đã được hash
    /// </summary>
    public string PasswordHash { get; private set; } = string.Empty;

    /// <summary>
    /// Họ và tên
    /// </summary>
    public string FullName { get; private set; } = string.Empty;

    /// <summary>
    /// Số điện thoại
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Địa chỉ giao hàng mặc định
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// URL ảnh đại diện
    /// </summary>
    public string? AvatarUrl { get; private set; }

    #endregion

    #region VIP & Loyalty Properties

    /// <summary>
    /// Cấp độ VIP - Quan trọng cho Strategy Pattern tính giá
    /// </summary>
    public VipLevel VipLevel { get; private set; } = VipLevel.None;

    /// <summary>
    /// Tổng chi tiêu tích lũy (VND) - Dùng để xác định VipLevel
    /// </summary>
    public decimal TotalSpent { get; private set; } = 0;

    /// <summary>
    /// Điểm thưởng tích lũy
    /// </summary>
    public int LoyaltyPoints { get; private set; } = 0;

    #endregion

    #region Skin Type Properties (AI Skin Quiz)

    /// <summary>
    /// Loại da của người dùng - Từ AI Skin Quiz
    /// Quan trọng cho SkinTypePricingStrategy
    /// </summary>
    public SkinType SkinType { get; private set; } = SkinType.Normal;

    /// <summary>
    /// Đã hoàn thành Skin Quiz chưa
    /// </summary>
    public bool HasCompletedSkinQuiz { get; private set; } = false;

    /// <summary>
    /// Ngày hoàn thành Skin Quiz gần nhất
    /// </summary>
    public DateTime? SkinQuizCompletedAt { get; private set; }

    #endregion

    #region Constructors

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected User() { }

    /// <summary>
    /// Constructor chính - Tạo user mới
    /// </summary>
    public User(string email, string passwordHash, string fullName)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email không được để trống", nameof(email));
        
        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Mật khẩu không được để trống", nameof(passwordHash));
        
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Họ tên không được để trống", nameof(fullName));

        Email = email.ToLower().Trim();
        PasswordHash = passwordHash;
        FullName = fullName.Trim();
    }

    #endregion

    #region Profile Update Methods

    /// <summary>
    /// Cập nhật thông tin cá nhân
    /// </summary>
    public void UpdateProfile(string? fullName = null, string? phoneNumber = null, 
                             string? address = null, string? avatarUrl = null)
    {
        if (!string.IsNullOrWhiteSpace(fullName)) FullName = fullName.Trim();
        if (phoneNumber != null) PhoneNumber = phoneNumber;
        if (address != null) Address = address;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Mật khẩu mới không được để trống", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region VIP & Loyalty Methods

    /// <summary>
    /// Thêm chi tiêu và tự động cập nhật VipLevel
    /// Gọi sau khi đơn hàng hoàn thành thanh toán
    /// </summary>
    /// <param name="amount">Số tiền chi tiêu</param>
    public void AddSpending(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Số tiền phải lớn hơn 0", nameof(amount));

        TotalSpent += amount;
        UpdateVipLevel();
        
        // Tích điểm: 1 điểm cho mỗi 10,000 VND
        LoyaltyPoints += (int)(amount / 10000);
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật VipLevel dựa trên tổng chi tiêu
    /// </summary>
    private void UpdateVipLevel()
    {
        VipLevel = TotalSpent switch
        {
            >= 20_000_000 => VipLevel.Platinum,
            >= 10_000_000 => VipLevel.Gold,
            >= 5_000_000 => VipLevel.Silver,
            >= 1_000_000 => VipLevel.Bronze,
            _ => VipLevel.None
        };
    }

    /// <summary>
    /// Sử dụng điểm thưởng
    /// </summary>
    /// <param name="points">Số điểm sử dụng</param>
    public void UsePoints(int points)
    {
        if (points <= 0)
            throw new ArgumentException("Số điểm phải lớn hơn 0", nameof(points));
        
        if (points > LoyaltyPoints)
            throw new InvalidOperationException($"Không đủ điểm. Hiện có: {LoyaltyPoints}, yêu cầu: {points}");

        LoyaltyPoints -= points;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Lấy phần trăm giảm giá theo VipLevel
    /// </summary>
    public decimal GetVipDiscountPercent()
    {
        return VipLevel switch
        {
            VipLevel.Platinum => 0.20m,  // 20%
            VipLevel.Gold => 0.15m,      // 15%
            VipLevel.Silver => 0.10m,    // 10%
            VipLevel.Bronze => 0.05m,    // 5%
            _ => 0m                       // 0%
        };
    }

    #endregion

    #region Skin Quiz Methods

    /// <summary>
    /// Hoàn thành Skin Quiz - Cập nhật loại da
    /// </summary>
    /// <param name="skinType">Loại da từ kết quả quiz</param>
    public void CompleteSkinQuiz(SkinType skinType)
    {
        SkinType = skinType;
        HasCompletedSkinQuiz = true;
        SkinQuizCompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kiểm tra xem loại da của user có phù hợp với sản phẩm không
    /// Dùng cho SkinTypePricingStrategy
    /// </summary>
    /// <param name="productSkinType">Loại da phù hợp của sản phẩm</param>
    /// <returns>True nếu phù hợp</returns>
    public bool IsSkinTypeMatch(SkinType productSkinType)
    {
        // Sản phẩm All phù hợp với mọi loại da
        if (productSkinType == SkinType.All) return true;
        
        // Nếu user chưa làm quiz thì không match
        if (!HasCompletedSkinQuiz) return false;
        
        return SkinType == productSkinType;
    }

    #endregion
}


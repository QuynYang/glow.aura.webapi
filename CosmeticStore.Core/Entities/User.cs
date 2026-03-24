using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Entities;


/// Entity đại diện cho người dùng/khách hàng.
/// Áp dụng tính ĐÓNG GÓI (Encapsulation) - Bảo vệ dữ liệu.
/// 
/// Thuộc tính quan trọng cho Strategy Pattern:
/// - VipLevel: Xác định mức giảm giá VIP
/// - SkinType: Xác định loại da để gợi ý sản phẩm và giảm giá

public class User : BaseEntity
{
    #region Basic Properties

    
    /// Email đăng nhập (unique)
    
    public string Email { get; private set; } = string.Empty;

    
    /// Mật khẩu đã được hash
    
    public string PasswordHash { get; private set; } = string.Empty;

    
    /// Họ và tên
    
    public string FullName { get; private set; } = string.Empty;

    
    /// Số điện thoại
    
    public string? PhoneNumber { get; private set; }

    
    /// Địa chỉ giao hàng mặc định
    
    public string? Address { get; private set; }

    
    /// URL ảnh đại diện
    
    public string? AvatarUrl { get; private set; }

    
    /// Vai trò người dùng (User, Staff, Admin)
    /// Quan trọng cho Authorization
    
    public UserRole Role { get; private set; } = UserRole.User;

    
    /// Trạng thái hoạt động của tài khoản
    
    public bool IsActive { get; private set; } = true;

    
    /// Thời gian đăng nhập lần cuối
    
    public DateTime? LastLoginAt { get; private set; }

    
    /// Refresh Token cho JWT (lưu để validate)
    
    public string? RefreshToken { get; private set; }
/// Giới tính
    public string? Gender { get; private set; }

    /// Ngày sinh
    public DateTime? DateOfBirth { get; private set; }
    
    /// Thời hạn Refresh Token
    
    public DateTime? RefreshTokenExpiryTime { get; private set; }

    #endregion

    #region VIP & Loyalty Properties

    
    /// Cấp độ VIP - Quan trọng cho Strategy Pattern tính giá
    
    public VipLevel VipLevel { get; private set; } = VipLevel.None;

    
    /// Tổng chi tiêu tích lũy (VND) - Dùng để xác định VipLevel
    
    public decimal TotalSpent { get; private set; } = 0;

    
    /// Điểm thưởng tích lũy
    
    public int LoyaltyPoints { get; private set; } = 0;

    #endregion

    #region Skin Type Properties (AI Skin Quiz)

    
    /// Loại da của người dùng - Từ AI Skin Quiz
    /// Quan trọng cho SkinTypePricingStrategy
    
    public SkinType SkinType { get; private set; } = SkinType.Normal;

    
    /// Đã hoàn thành Skin Quiz chưa
    
    public bool HasCompletedSkinQuiz { get; private set; } = false;

    
    /// Ngày hoàn thành Skin Quiz gần nhất
    
    public DateTime? SkinQuizCompletedAt { get; private set; }

    #endregion

    #region Constructors

    
    /// Constructor mặc định cho EF Core
    
    protected User() { }

    
    /// Constructor chính - Tạo user mới
    
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
        Role = UserRole.User; // Mặc định là User
    }

    
    /// Constructor với Role - Dùng để tạo Admin/Staff
    
    public User(string email, string passwordHash, string fullName, UserRole role) 
        : this(email, passwordHash, fullName)
    {
        Role = role;
    }

    #endregion

    #region Authentication Methods

    
    /// Cập nhật thời gian đăng nhập
    
    public void RecordLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Cập nhật Refresh Token
    
    public void SetRefreshToken(string refreshToken, DateTime expiryTime)
    {
        RefreshToken = refreshToken;
        RefreshTokenExpiryTime = expiryTime;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Xóa Refresh Token (Logout)
    
    public void RevokeRefreshToken()
    {
        RefreshToken = null;
        RefreshTokenExpiryTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Kiểm tra Refresh Token còn hợp lệ không
    
    public bool IsRefreshTokenValid(string token)
    {
        return RefreshToken == token && 
               RefreshTokenExpiryTime.HasValue && 
               RefreshTokenExpiryTime.Value > DateTime.UtcNow;
    }

    
    /// Kích hoạt/Vô hiệu hóa tài khoản
    
    public void SetActiveStatus(bool isActive)
    {
        IsActive = isActive;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Thay đổi vai trò (chỉ Admin mới được gọi)
    
    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Kiểm tra có phải Admin không
    
    public bool IsAdmin => Role == UserRole.Admin;

    
    /// Kiểm tra có phải Staff hoặc Admin không
    
    public bool IsStaffOrAdmin => Role == UserRole.Staff || Role == UserRole.Admin;

    #endregion

    #region Profile Update Methods

    
    /// Cập nhật thông tin cá nhân
    public void UpdateProfile(string? fullName = null, string? phoneNumber = null, 
                              string? address = null, string? avatarUrl = null,
                              string? gender = null, DateTime? dateOfBirth = null) // Bổ sung 2 tham số này
    {
        if (!string.IsNullOrWhiteSpace(fullName)) FullName = fullName.Trim();
        if (phoneNumber != null) PhoneNumber = phoneNumber;
        if (address != null) Address = address;
        if (avatarUrl != null) AvatarUrl = avatarUrl;
        
        // Cập nhật giá trị mới
        if (gender != null) Gender = gender;
        if (dateOfBirth != null) DateOfBirth = dateOfBirth;
        
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Đổi mật khẩu
    
    public void ChangePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Mật khẩu mới không được để trống", nameof(newPasswordHash));
        
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region VIP & Loyalty Methods

    
    /// Thêm chi tiêu và tự động cập nhật VipLevel
    /// Gọi sau khi đơn hàng hoàn thành thanh toán
    
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

    
    /// Cập nhật VipLevel dựa trên tổng chi tiêu
    
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

    
    /// Sử dụng điểm thưởng
    
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

    
    /// Lấy phần trăm giảm giá theo VipLevel
    
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

    
    /// Hoàn thành Skin Quiz - Cập nhật loại da
    
    /// <param name="skinType">Loại da từ kết quả quiz</param>
    public void CompleteSkinQuiz(SkinType skinType)
    {
        SkinType = skinType;
        HasCompletedSkinQuiz = true;
        SkinQuizCompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Kiểm tra xem loại da của user có phù hợp với sản phẩm không
    /// Dùng cho SkinTypePricingStrategy
    
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


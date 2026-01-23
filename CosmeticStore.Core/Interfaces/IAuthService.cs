using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho Authentication Service
/// Xử lý đăng ký, đăng nhập, JWT token
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? phoneNumber = null);

    /// <summary>
    /// Đăng nhập
    /// </summary>
    Task<AuthResult> LoginAsync(string email, string password);

    /// <summary>
    /// Refresh Access Token
    /// </summary>
    Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken);

    /// <summary>
    /// Đăng xuất (revoke refresh token)
    /// </summary>
    Task<bool> LogoutAsync(int userId);

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    /// <summary>
    /// Hash mật khẩu
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Xác thực mật khẩu
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);

    /// <summary>
    /// Tạo Access Token
    /// </summary>
    string GenerateAccessToken(User user);

    /// <summary>
    /// Tạo Refresh Token
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Lấy User ID từ expired token (dùng cho refresh)
    /// </summary>
    int? GetUserIdFromExpiredToken(string token);
}

/// <summary>
/// Kết quả Authentication
/// </summary>
public class AuthResult
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public User? User { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiry { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public static AuthResult Success(User user, string accessToken, string refreshToken, 
        DateTime accessExpiry, DateTime refreshExpiry, string message = "Thành công")
    {
        return new AuthResult
        {
            IsSuccess = true,
            Message = message,
            User = user,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            AccessTokenExpiry = accessExpiry,
            RefreshTokenExpiry = refreshExpiry
        };
    }

    public static AuthResult Fail(string message)
    {
        return new AuthResult
        {
            IsSuccess = false,
            Message = message
        };
    }
}


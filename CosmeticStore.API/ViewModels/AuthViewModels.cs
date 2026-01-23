using System.ComponentModel.DataAnnotations;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.API.ViewModels;

#region Request Models

/// <summary>
/// Request đăng ký tài khoản mới
/// </summary>
public record RegisterRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(100, ErrorMessage = "Họ tên không được quá 100 ký tự")]
    public string FullName { get; init; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; init; }
}

/// <summary>
/// Request đăng nhập
/// </summary>
public record LoginRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// Request refresh token
/// </summary>
public record RefreshTokenRequest
{
    [Required(ErrorMessage = "Access Token là bắt buộc")]
    public string AccessToken { get; init; } = string.Empty;

    [Required(ErrorMessage = "Refresh Token là bắt buộc")]
    public string RefreshToken { get; init; } = string.Empty;
}

/// <summary>
/// Request đổi mật khẩu
/// </summary>
public record ChangePasswordRequest
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
    public string CurrentPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string NewPassword { get; init; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmNewPassword { get; init; } = string.Empty;
}

#endregion

#region Response Models

/// <summary>
/// Response sau khi đăng nhập/đăng ký thành công
/// </summary>
public record AuthResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public TokenResponse? Token { get; init; }
    public UserResponse? User { get; init; }
}

/// <summary>
/// Token response
/// </summary>
public record TokenResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiry { get; init; }
    public DateTime RefreshTokenExpiry { get; init; }
    public string TokenType { get; init; } = "Bearer";
}

/// <summary>
/// Thông tin user trả về
/// </summary>
public record UserResponse
{
    public int Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? Address { get; init; }
    public string? AvatarUrl { get; init; }
    public string Role { get; init; } = string.Empty;
    public string VipLevel { get; init; } = string.Empty;
    public decimal TotalSpent { get; init; }
    public int LoyaltyPoints { get; init; }
    public string? SkinType { get; init; }
    public bool HasCompletedSkinQuiz { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}

/// <summary>
/// Response cơ bản
/// </summary>
public record ApiResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public object? Data { get; init; }

    public static ApiResponse Success(string message, object? data = null)
        => new() { IsSuccess = true, Message = message, Data = data };

    public static ApiResponse Fail(string message)
        => new() { IsSuccess = false, Message = message };
}

#endregion

#region Admin Request Models

/// <summary>
/// Request tạo user bởi Admin
/// </summary>
public record CreateUserRequest
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    public string FullName { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; }

    public UserRole Role { get; init; } = UserRole.User;
}

/// <summary>
/// Request cập nhật profile
/// </summary>
public record UpdateProfileRequest
{
    [MaxLength(100)]
    public string? FullName { get; init; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; init; }

    [MaxLength(500)]
    public string? Address { get; init; }

    [Url(ErrorMessage = "URL ảnh đại diện không hợp lệ")]
    public string? AvatarUrl { get; init; }
}

/// <summary>
/// Request thay đổi role (Admin only)
/// </summary>
public record ChangeRoleRequest
{
    [Required]
    public UserRole NewRole { get; init; }
}

#endregion


using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// Authentication Service - Xử lý đăng ký, đăng nhập, JWT
/// 
/// Tính năng:
/// - Hash password với BCrypt (hoặc PBKDF2)
/// - Tạo JWT Access Token
/// - Tạo Refresh Token
/// - Validate và Refresh token
/// </summary>
public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IConfiguration _configuration;
    private readonly ISystemLogger _logger;

    // JWT Settings
    private readonly string _jwtSecret;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _accessTokenExpiryMinutes;
    private readonly int _refreshTokenExpiryDays;

    public AuthService(
        IGenericRepository<User> userRepository,
        IConfiguration configuration,
        ISystemLogger logger)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _logger = logger;

        // Load JWT settings từ configuration
        _jwtSecret = _configuration["Jwt:Secret"] ?? "CosmeticStore_SuperSecretKey_12345678901234567890";
        _jwtIssuer = _configuration["Jwt:Issuer"] ?? "CosmeticStore.API";
        _jwtAudience = _configuration["Jwt:Audience"] ?? "CosmeticStore.Client";
        _accessTokenExpiryMinutes = int.Parse(_configuration["Jwt:AccessTokenExpiryMinutes"] ?? "60");
        _refreshTokenExpiryDays = int.Parse(_configuration["Jwt:RefreshTokenExpiryDays"] ?? "7");
    }

    #region Public Methods

    /// <summary>
    /// Đăng ký user mới
    /// </summary>
    public async Task<AuthResult> RegisterAsync(string email, string password, string fullName, string? phoneNumber = null)
    {
        try
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUsers = await _userRepository.FindAsync(u => u.Email == email.ToLower().Trim());
            if (existingUsers.Any())
            {
                return AuthResult.Fail("Email đã được sử dụng");
            }

            // Hash password
            var passwordHash = HashPassword(password);

            // Tạo user mới
            var user = new User(email, passwordHash, fullName);
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                user.UpdateProfile(phoneNumber: phoneNumber);
            }

            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            // Tạo tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var accessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
            var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            // Lưu refresh token
            user.SetRefreshToken(refreshToken, refreshExpiry);
            user.RecordLogin();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInfo($"[Auth] User registered: {email}", new { UserId = user.Id, Email = email });

            return AuthResult.Success(user, accessToken, refreshToken, accessExpiry, refreshExpiry, "Đăng ký thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Auth] Register failed for {email}: {ex.Message}", ex);
            return AuthResult.Fail("Đăng ký thất bại: " + ex.Message);
        }
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        try
        {
            // Tìm user
            var users = await _userRepository.FindAsync(u => u.Email == email.ToLower().Trim());
            var user = users.FirstOrDefault();

            if (user == null)
            {
                return AuthResult.Fail("Email hoặc mật khẩu không đúng");
            }

            // Kiểm tra tài khoản có bị khóa không
            if (!user.IsActive)
            {
                return AuthResult.Fail("Tài khoản đã bị vô hiệu hóa");
            }

            // Xác thực password
            if (!VerifyPassword(password, user.PasswordHash))
            {
                return AuthResult.Fail("Email hoặc mật khẩu không đúng");
            }

            // Tạo tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();
            var accessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
            var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            // Lưu refresh token và cập nhật last login
            user.SetRefreshToken(refreshToken, refreshExpiry);
            user.RecordLogin();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInfo($"[Auth] User logged in: {email}", new { UserId = user.Id });

            return AuthResult.Success(user, accessToken, refreshToken, accessExpiry, refreshExpiry, "Đăng nhập thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Auth] Login failed for {email}: {ex.Message}", ex);
            return AuthResult.Fail("Đăng nhập thất bại: " + ex.Message);
        }
    }

    /// <summary>
    /// Refresh Access Token
    /// </summary>
    public async Task<AuthResult> RefreshTokenAsync(string accessToken, string refreshToken)
    {
        try
        {
            // Lấy user id từ expired token
            var userId = GetUserIdFromExpiredToken(accessToken);
            if (userId == null)
            {
                return AuthResult.Fail("Access Token không hợp lệ");
            }

            // Tìm user
            var user = await _userRepository.GetByIdAsync(userId.Value);
            if (user == null)
            {
                return AuthResult.Fail("Người dùng không tồn tại");
            }

            // Kiểm tra refresh token
            if (!user.IsRefreshTokenValid(refreshToken))
            {
                return AuthResult.Fail("Refresh Token không hợp lệ hoặc đã hết hạn");
            }

            // Tạo tokens mới
            var newAccessToken = GenerateAccessToken(user);
            var newRefreshToken = GenerateRefreshToken();
            var accessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes);
            var refreshExpiry = DateTime.UtcNow.AddDays(_refreshTokenExpiryDays);

            // Lưu refresh token mới
            user.SetRefreshToken(newRefreshToken, refreshExpiry);
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInfo($"[Auth] Token refreshed for user {user.Id}");

            return AuthResult.Success(user, newAccessToken, newRefreshToken, accessExpiry, refreshExpiry, "Refresh token thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Auth] Refresh token failed: {ex.Message}", ex);
            return AuthResult.Fail("Refresh token thất bại: " + ex.Message);
        }
    }

    /// <summary>
    /// Đăng xuất
    /// </summary>
    public async Task<bool> LogoutAsync(int userId)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            user.RevokeRefreshToken();
            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInfo($"[Auth] User logged out: {userId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Auth] Logout failed for user {userId}: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            // Xác thực mật khẩu hiện tại
            if (!VerifyPassword(currentPassword, user.PasswordHash))
            {
                return false;
            }

            // Đổi mật khẩu
            var newHash = HashPassword(newPassword);
            user.ChangePassword(newHash);
            
            // Revoke tất cả refresh token (bắt buộc đăng nhập lại)
            user.RevokeRefreshToken();

            _userRepository.Update(user);
            await _userRepository.SaveChangesAsync();

            _logger.LogInfo($"[Auth] Password changed for user {userId}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError($"[Auth] Change password failed for user {userId}: {ex.Message}", ex);
            return false;
        }
    }

    #endregion

    #region Password Hashing

    /// <summary>
    /// Hash password sử dụng PBKDF2
    /// </summary>
    public string HashPassword(string password)
    {
        // Sử dụng PBKDF2 với salt
        using var rng = RandomNumberGenerator.Create();
        var salt = new byte[16];
        rng.GetBytes(salt);

        using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
        var hash = pbkdf2.GetBytes(32);

        // Kết hợp salt + hash
        var hashBytes = new byte[48];
        Array.Copy(salt, 0, hashBytes, 0, 16);
        Array.Copy(hash, 0, hashBytes, 16, 32);

        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Xác thực password
    /// </summary>
    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var hashBytes = Convert.FromBase64String(passwordHash);
            
            // Tách salt
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);

            // Hash password với salt
            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 100000, HashAlgorithmName.SHA256);
            var hash = pbkdf2.GetBytes(32);

            // So sánh
            for (int i = 0; i < 32; i++)
            {
                if (hashBytes[i + 16] != hash[i])
                    return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region JWT Token Generation

    /// <summary>
    /// Tạo Access Token
    /// </summary>
    public string GenerateAccessToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim("VipLevel", user.VipLevel.ToString()),
            new Claim("SkinType", user.SkinType.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        var token = new JwtSecurityToken(
            issuer: _jwtIssuer,
            audience: _jwtAudience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Tạo Refresh Token
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Lấy User ID từ expired token
    /// </summary>
    public int? GetUserIdFromExpiredToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret)),
                ValidateLifetime = false // Cho phép token hết hạn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }

            return null;
        }
        catch
        {
            return null;
        }
    }

    #endregion
}


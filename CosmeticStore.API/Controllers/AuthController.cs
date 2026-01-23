using CosmeticStore.API.ViewModels;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// Controller xử lý Authentication (Đăng ký, Đăng nhập, Token)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ISystemLogger _logger;

    public AuthController(IAuthService authService, ISystemLogger logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// </summary>
    /// <param name="request">Thông tin đăng ký</param>
    /// <returns>Token và thông tin user</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/register
    ///     {
    ///         "email": "user@example.com",
    ///         "password": "Password123",
    ///         "confirmPassword": "Password123",
    ///         "fullName": "Nguyễn Văn A",
    ///         "phoneNumber": "0901234567"
    ///     }
    /// </remarks>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                IsSuccess = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var result = await _authService.RegisterAsync(
            request.Email,
            request.Password,
            request.FullName,
            request.PhoneNumber
        );

        if (!result.IsSuccess)
        {
            return BadRequest(new AuthResponse
            {
                IsSuccess = false,
                Message = result.Message
            });
        }

        return Ok(new AuthResponse
        {
            IsSuccess = true,
            Message = result.Message,
            Token = new TokenResponse
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                AccessTokenExpiry = result.AccessTokenExpiry!.Value,
                RefreshTokenExpiry = result.RefreshTokenExpiry!.Value
            },
            User = MapToUserResponse(result.User!)
        });
    }

    /// <summary>
    /// Đăng nhập
    /// </summary>
    /// <param name="request">Email và mật khẩu</param>
    /// <returns>Token và thông tin user</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "email": "user@example.com",
    ///         "password": "Password123"
    ///     }
    /// </remarks>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                IsSuccess = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var result = await _authService.LoginAsync(request.Email, request.Password);

        if (!result.IsSuccess)
        {
            return Unauthorized(new AuthResponse
            {
                IsSuccess = false,
                Message = result.Message
            });
        }

        return Ok(new AuthResponse
        {
            IsSuccess = true,
            Message = result.Message,
            Token = new TokenResponse
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                AccessTokenExpiry = result.AccessTokenExpiry!.Value,
                RefreshTokenExpiry = result.RefreshTokenExpiry!.Value
            },
            User = MapToUserResponse(result.User!)
        });
    }

    /// <summary>
    /// Refresh Access Token
    /// </summary>
    /// <param name="request">Access Token (hết hạn) và Refresh Token</param>
    /// <returns>Token mới</returns>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new AuthResponse
            {
                IsSuccess = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);

        if (!result.IsSuccess)
        {
            return Unauthorized(new AuthResponse
            {
                IsSuccess = false,
                Message = result.Message
            });
        }

        return Ok(new AuthResponse
        {
            IsSuccess = true,
            Message = result.Message,
            Token = new TokenResponse
            {
                AccessToken = result.AccessToken!,
                RefreshToken = result.RefreshToken!,
                AccessTokenExpiry = result.AccessTokenExpiry!.Value,
                RefreshTokenExpiry = result.RefreshTokenExpiry!.Value
            },
            User = MapToUserResponse(result.User!)
        });
    }

    /// <summary>
    /// Đăng xuất (Revoke Refresh Token)
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse>> Logout()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được người dùng"));
        }

        var success = await _authService.LogoutAsync(userId.Value);

        if (!success)
        {
            return BadRequest(ApiResponse.Fail("Đăng xuất thất bại"));
        }

        return Ok(ApiResponse.Success("Đăng xuất thành công"));
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được người dùng"));
        }

        var success = await _authService.ChangePasswordAsync(
            userId.Value,
            request.CurrentPassword,
            request.NewPassword
        );

        if (!success)
        {
            return BadRequest(ApiResponse.Fail("Mật khẩu hiện tại không đúng"));
        }

        return Ok(ApiResponse.Success("Đổi mật khẩu thành công. Vui lòng đăng nhập lại."));
    }

    #region Helper Methods

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
        {
            return userId;
        }
        return null;
    }

    private static UserResponse MapToUserResponse(User user)
    {
        return new UserResponse
        {
            Id = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber,
            Address = user.Address,
            AvatarUrl = user.AvatarUrl,
            Role = user.Role.ToString(),
            VipLevel = user.VipLevel.ToString(),
            TotalSpent = user.TotalSpent,
            LoyaltyPoints = user.LoyaltyPoints,
            SkinType = user.HasCompletedSkinQuiz ? user.SkinType.ToString() : null,
            HasCompletedSkinQuiz = user.HasCompletedSkinQuiz,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt
        };
    }

    #endregion
}


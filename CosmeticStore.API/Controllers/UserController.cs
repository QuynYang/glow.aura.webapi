using CosmeticStore.API.ViewModels;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// Controller quản lý User (Profile, Admin quản lý users)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IGenericRepository<User> _userRepository;
    private readonly IAuthService _authService;
    private readonly ISystemLogger _logger;

    public UserController(
        IGenericRepository<User> userRepository,
        IAuthService authService,
        ISystemLogger logger)
    {
        _userRepository = userRepository;
        _authService = authService;
        _logger = logger;
    }

    #region Current User Endpoints

    /// <summary>
    /// Lấy thông tin profile của user đang đăng nhập
    /// </summary>
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được người dùng"));
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        return Ok(MapToUserResponse(user));
    }

    /// <summary>
    /// Cập nhật profile của user đang đăng nhập
    /// </summary>
    [HttpPut("me")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserResponse>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(ApiResponse.Fail("Không xác định được người dùng"));
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        user.UpdateProfile(
            fullName: request.FullName,
            phoneNumber: request.PhoneNumber,
            address: request.Address,
            avatarUrl: request.AvatarUrl
        );

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInfo($"User {userId} updated profile", "User");

        return Ok(MapToUserResponse(user));
    }

    /// <summary>
    /// Lấy thông tin VIP và điểm thưởng
    /// </summary>
    [HttpGet("me/loyalty")]
    [ProducesResponseType(typeof(LoyaltyInfo), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoyaltyInfo>> GetLoyaltyInfo()
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(new LoyaltyInfo
        {
            VipLevel = user.VipLevel.ToString(),
            VipDiscountPercent = user.GetVipDiscountPercent() * 100,
            TotalSpent = user.TotalSpent,
            LoyaltyPoints = user.LoyaltyPoints,
            NextVipLevel = GetNextVipLevel(user.VipLevel),
            AmountToNextLevel = GetAmountToNextLevel(user)
        });
    }

    #endregion

    #region Admin Endpoints

    /// <summary>
    /// [Admin] Lấy danh sách tất cả users
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAllUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var users = await _userRepository.GetAllAsync();
        var pagedUsers = users
            .OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToUserResponse);

        return Ok(new
        {
            Data = pagedUsers,
            Page = page,
            PageSize = pageSize,
            TotalCount = users.Count()
        });
    }

    /// <summary>
    /// [Admin] Lấy thông tin chi tiết một user
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetUserById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        return Ok(MapToUserResponse(user));
    }

    /// <summary>
    /// [Admin] Tạo user mới (có thể chỉ định role)
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponse>> CreateUser([FromBody] CreateUserRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ApiResponse.Fail("Dữ liệu không hợp lệ"));
        }

        // Kiểm tra email đã tồn tại chưa
        var existingUsers = await _userRepository.FindAsync(u => u.Email == request.Email.ToLower().Trim());
        if (existingUsers.Any())
        {
            return BadRequest(ApiResponse.Fail("Email đã được sử dụng"));
        }

        // Hash password và tạo user
        var passwordHash = _authService.HashPassword(request.Password);
        var user = new User(request.Email, passwordHash, request.FullName, request.Role);

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            user.UpdateProfile(phoneNumber: request.PhoneNumber);
        }

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInfo($"Admin created user: {request.Email} with role {request.Role}", "User");

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, MapToUserResponse(user));
    }

    /// <summary>
    /// [Admin] Thay đổi role của user
    /// </summary>
    [HttpPatch("{id}/role")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> ChangeUserRole(int id, [FromBody] ChangeRoleRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        // Không cho phép tự thay đổi role của chính mình
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id)
        {
            return BadRequest(ApiResponse.Fail("Không thể thay đổi role của chính mình"));
        }

        user.ChangeRole(request.NewRole);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInfo($"Admin changed role of user {id} to {request.NewRole}", "User");

        return Ok(MapToUserResponse(user));
    }

    /// <summary>
    /// [Admin] Kích hoạt/Vô hiệu hóa tài khoản
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> SetUserStatus(int id, [FromQuery] bool isActive)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        // Không cho phép tự vô hiệu hóa chính mình
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id && !isActive)
        {
            return BadRequest(ApiResponse.Fail("Không thể vô hiệu hóa chính mình"));
        }

        user.SetActiveStatus(isActive);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        var message = isActive ? "Đã kích hoạt tài khoản" : "Đã vô hiệu hóa tài khoản";
        _logger.LogInfo($"Admin {(isActive ? "activated" : "deactivated")} user {id}", "User");

        return Ok(ApiResponse.Success(message));
    }

    /// <summary>
    /// [Admin] Xóa user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse>> DeleteUser(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse.Fail("Người dùng không tồn tại"));
        }

        // Không cho phép tự xóa chính mình
        var currentUserId = GetCurrentUserId();
        if (currentUserId == id)
        {
            return BadRequest(ApiResponse.Fail("Không thể xóa chính mình"));
        }

        _userRepository.SoftDelete(user);
        await _userRepository.SaveChangesAsync();

        _logger.LogInfo($"Admin deleted user {id}", "User");

        return Ok(ApiResponse.Success("Đã xóa người dùng"));
    }

    /// <summary>
    /// [Admin] Thống kê users
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(UserStats), StatusCodes.Status200OK)]
    public async Task<ActionResult<UserStats>> GetUserStats()
    {
        var users = await _userRepository.GetAllAsync();
        var userList = users.ToList();

        return Ok(new UserStats
        {
            TotalUsers = userList.Count,
            ActiveUsers = userList.Count(u => u.IsActive),
            InactiveUsers = userList.Count(u => !u.IsActive),
            AdminCount = userList.Count(u => u.Role == UserRole.Admin),
            StaffCount = userList.Count(u => u.Role == UserRole.Staff),
            CustomerCount = userList.Count(u => u.Role == UserRole.User),
            VipDistribution = new Dictionary<string, int>
            {
                { "None", userList.Count(u => u.VipLevel == VipLevel.None) },
                { "Bronze", userList.Count(u => u.VipLevel == VipLevel.Bronze) },
                { "Silver", userList.Count(u => u.VipLevel == VipLevel.Silver) },
                { "Gold", userList.Count(u => u.VipLevel == VipLevel.Gold) },
                { "Platinum", userList.Count(u => u.VipLevel == VipLevel.Platinum) }
            },
            NewUsersThisMonth = userList.Count(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30))
        });
    }

    #endregion

    #region Helper Methods

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
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

    private static string? GetNextVipLevel(VipLevel currentLevel)
    {
        return currentLevel switch
        {
            VipLevel.None => "Bronze",
            VipLevel.Bronze => "Silver",
            VipLevel.Silver => "Gold",
            VipLevel.Gold => "Platinum",
            VipLevel.Platinum => null, // Đã max
            _ => null
        };
    }

    private static decimal? GetAmountToNextLevel(User user)
    {
        return user.VipLevel switch
        {
            VipLevel.None => 1_000_000 - user.TotalSpent,
            VipLevel.Bronze => 5_000_000 - user.TotalSpent,
            VipLevel.Silver => 10_000_000 - user.TotalSpent,
            VipLevel.Gold => 20_000_000 - user.TotalSpent,
            VipLevel.Platinum => null, // Đã max
            _ => null
        };
    }

    #endregion
}

#region Response DTOs

public record LoyaltyInfo
{
    public string VipLevel { get; init; } = string.Empty;
    public decimal VipDiscountPercent { get; init; }
    public decimal TotalSpent { get; init; }
    public int LoyaltyPoints { get; init; }
    public string? NextVipLevel { get; init; }
    public decimal? AmountToNextLevel { get; init; }
}

public record UserStats
{
    public int TotalUsers { get; init; }
    public int ActiveUsers { get; init; }
    public int InactiveUsers { get; init; }
    public int AdminCount { get; init; }
    public int StaffCount { get; init; }
    public int CustomerCount { get; init; }
    public Dictionary<string, int> VipDistribution { get; init; } = new();
    public int NewUsersThisMonth { get; init; }
}

#endregion


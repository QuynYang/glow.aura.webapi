using CosmeticStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CosmeticStore.API.Controllers;


/// FACADE PATTERN - Controller checkout đơn giản
/// Controller này chỉ cần inject 1 service duy nhất: ICheckoutFacade
/// Facade ẩn toàn bộ sự phức tạp bên trong (Pricing, Order, Payment, Notification)

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckoutController : ControllerBase
{
    // Chỉ cần 1 dependency duy nhất - đây là ưu điểm của Facade Pattern
    private readonly ICheckoutFacade _checkoutFacade;

    public CheckoutController(ICheckoutFacade checkoutFacade)
    {
        _checkoutFacade = checkoutFacade;
    }

    
    /// Xem trước đơn hàng trước khi thanh toán
    /// Chỉ tính giá, KHÔNG tạo đơn thật
    /// POST: api/checkout/preview
    
    [HttpPost("preview")]
    [ProducesResponseType(typeof(CheckoutPreview), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckoutPreview>> Preview([FromBody] CheckoutRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        request.UserId = userId.Value;

        // Facade xử lý tất cả - Controller không cần biết chi tiết
        var preview = await _checkoutFacade.PreviewCheckoutAsync(request);

        if (!preview.IsValid)
            return BadRequest(preview);

        return Ok(preview);
    }

    
    /// Thực hiện checkout - Tạo đơn + Thanh toán + Gửi thông báo
    /// Tất cả trong 1 lần gọi nhờ Facade Pattern
    /// POST: api/checkout
    
    [HttpPost]
    [ProducesResponseType(typeof(CheckoutResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CheckoutResult), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CheckoutResult>> Checkout([FromBody] CheckoutRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null) return Unauthorized();

        request.UserId = userId.Value;

        // 1 dòng duy nhất - Facade xử lý toàn bộ quy trình phức tạp bên trong
        var result = await _checkoutFacade.ProcessCheckoutAsync(request);

        if (!result.IsSuccess)
            return BadRequest(result);

        return CreatedAtAction(nameof(Checkout), new { id = result.OrderId }, result);
    }

    #region Helper

    private int? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            return userId;
        return null;
    }

    #endregion
}

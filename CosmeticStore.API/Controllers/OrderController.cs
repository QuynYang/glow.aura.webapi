using CosmeticStore.API.ViewModels;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CosmeticStore.API.Controllers;

/// <summary>
/// Controller quản lý đơn hàng
/// Sử dụng Command Pattern cho các thao tác write
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICommandHandler<CreateOrderCommand, CreateOrderResult> _createOrderHandler;
    private readonly ICommandHandler<CancelOrderCommand, CancelOrderResult> _cancelOrderHandler;
    private readonly ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult> _confirmOrderHandler;
    private readonly ICommandHandler<PayOrderCommand, PayOrderResult> _payOrderHandler;
    private readonly ISystemLogger _logger;

    public OrderController(
        IOrderRepository orderRepository,
        ICommandHandler<CreateOrderCommand, CreateOrderResult> createOrderHandler,
        ICommandHandler<CancelOrderCommand, CancelOrderResult> cancelOrderHandler,
        ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult> confirmOrderHandler,
        ICommandHandler<PayOrderCommand, PayOrderResult> payOrderHandler,
        ISystemLogger logger)
    {
        _orderRepository = orderRepository;
        _createOrderHandler = createOrderHandler;
        _cancelOrderHandler = cancelOrderHandler;
        _confirmOrderHandler = confirmOrderHandler;
        _payOrderHandler = payOrderHandler;
        _logger = logger;
    }

    #region Customer Endpoints

    /// <summary>
    /// Tạo đơn hàng mới
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(CreateOrderResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new CreateOrderResponse
            {
                IsSuccess = false,
                Message = "Dữ liệu không hợp lệ"
            });
        }

        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized(new CreateOrderResponse
            {
                IsSuccess = false,
                Message = "Không xác định được người dùng"
            });
        }

        // Chuyển đổi từ Request sang Command
        var items = request.Items.Select(i => new OrderItemInput 
        { 
            ProductId = i.ProductId, 
            Quantity = i.Quantity 
        });

        var command = new CreateOrderCommand(
            userId.Value,
            items,
            request.ShippingAddress,
            request.ShippingPhone,
            request.ReceiverName,
            request.PaymentMethod,
            request.Notes,
            request.CouponCode
        );

        // Execute Command
        var result = await _createOrderHandler.HandleAsync(command);

        if (!result.IsSuccess)
        {
            return BadRequest(new CreateOrderResponse
            {
                IsSuccess = false,
                Message = result.ErrorMessage ?? "Tạo đơn hàng thất bại"
            });
        }

        return CreatedAtAction(nameof(GetOrderById), new { id = result.Data!.OrderId }, new CreateOrderResponse
        {
            IsSuccess = true,
            Message = result.Data.Message,
            OrderId = result.Data.OrderId,
            OrderNumber = result.Data.OrderNumber,
            TotalAmount = result.Data.TotalAmount,
            PaymentUrl = result.Data.PaymentUrl
        });
    }

    /// <summary>
    /// Lấy danh sách đơn hàng của user hiện tại
    /// </summary>
    [HttpGet("my-orders")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetMyOrders([FromQuery] OrderStatus? status = null)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var orders = await _orderRepository.GetByUserIdAsync(userId.Value);

        // Filter theo status nếu có
        if (status.HasValue)
        {
            orders = orders.Where(o => o.Status == status.Value);
        }

        return Ok(orders.Select(MapToOrderSummary));
    }

    /// <summary>
    /// Xem chi tiết đơn hàng (user chỉ xem được đơn của mình)
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetOrderById(int id)
    {
        var order = await _orderRepository.GetWithItemsAsync(id);
        if (order == null)
        {
            return NotFound(ApiResponse.Fail("Đơn hàng không tồn tại"));
        }

        // Kiểm tra quyền: User chỉ xem đơn của mình, Staff/Admin xem tất cả
        var userId = GetCurrentUserId();
        var isStaffOrAdmin = User.IsInRole("Admin") || User.IsInRole("Staff");

        if (!isStaffOrAdmin && order.UserId != userId)
        {
            return Forbid();
        }

        return Ok(MapToOrderResponse(order));
    }

    /// <summary>
    /// Hủy đơn hàng (chỉ hủy được khi đang Pending hoặc Confirmed)
    /// </summary>
    [HttpPost("{id}/cancel")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> CancelOrder(int id, [FromBody] CancelOrderRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var command = new CancelOrderCommand(id, request.Reason, userId.Value, false);
        var result = await _cancelOrderHandler.HandleAsync(command);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse.Fail(result.ErrorMessage ?? "Hủy đơn hàng thất bại"));
        }

        return Ok(ApiResponse.Success(result.Data!.Message));
    }

    /// <summary>
    /// Thanh toán đơn hàng
    /// </summary>
    [HttpPost("{id}/pay")]
    [ProducesResponseType(typeof(PaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentResponse>> PayOrder(int id, [FromBody] PayOrderRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        // Verify user owns this order
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null || order.UserId != userId)
        {
            return NotFound(ApiResponse.Fail("Đơn hàng không tồn tại"));
        }

        var command = new PayOrderCommand(
            id, 
            request.PaymentMethod,
            request.ReturnUrl
        );
        
        var result = await _payOrderHandler.HandleAsync(command);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse.Fail(result.ErrorMessage ?? "Thanh toán thất bại"));
        }

        return Ok(new PaymentResponse
        {
            IsSuccess = true,
            Message = result.Data!.Message,
            OrderId = id,
            TransactionId = result.Data.TransactionId,
            RedirectUrl = result.Data.PaymentUrl,
            QrCodeData = result.Data.QrCode
        });
    }

    #endregion

    #region Admin/Staff Endpoints

    /// <summary>
    /// [Admin/Staff] Lấy tất cả đơn hàng
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetAllOrders(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] OrderStatus? status = null)
    {
        IEnumerable<Order> orders;

        if (status.HasValue)
        {
            orders = await _orderRepository.GetByStatusAsync(status.Value);
        }
        else
        {
            orders = await _orderRepository.GetAllAsync();
        }

        var pagedOrders = orders
            .OrderByDescending(o => o.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return Ok(new
        {
            Data = pagedOrders.Select(MapToOrderSummary),
            Page = page,
            PageSize = pageSize,
            TotalCount = orders.Count()
        });
    }

    /// <summary>
    /// [Admin/Staff] Xác nhận đơn hàng
    /// </summary>
    [HttpPost("{id}/confirm")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> ConfirmOrder(int id, [FromBody] ConfirmOrderRequest request)
    {
        var userId = GetCurrentUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        var command = new ConfirmOrderCommand(id, userId.Value, request.ShippingFee);
        var result = await _confirmOrderHandler.HandleAsync(command);

        if (!result.IsSuccess)
        {
            return BadRequest(ApiResponse.Fail(result.ErrorMessage ?? "Xác nhận đơn hàng thất bại"));
        }

        return Ok(ApiResponse.Success(result.Data!.Message));
    }

    /// <summary>
    /// [Admin/Staff] Cập nhật trạng thái đơn hàng
    /// </summary>
    [HttpPatch("{id}/status")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApiResponse>> UpdateOrderStatus(int id, [FromQuery] OrderStatus newStatus)
    {
        var order = await _orderRepository.GetByIdAsync(id);
        if (order == null)
        {
            return NotFound(ApiResponse.Fail("Đơn hàng không tồn tại"));
        }

        try
        {
            switch (newStatus)
            {
                case OrderStatus.Processing:
                    order.StartProcessing();
                    break;
                case OrderStatus.Shipping:
                    order.StartShipping();
                    break;
                case OrderStatus.Delivered:
                    order.MarkAsDelivered();
                    break;
                case OrderStatus.Completed:
                    order.Complete();
                    break;
                default:
                    return BadRequest(ApiResponse.Fail($"Không hỗ trợ chuyển sang trạng thái {newStatus}"));
            }

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            _logger.LogOrderActivity(order.Id, OrderActivityType.Updated, 
                $"Order status updated to {newStatus}", GetCurrentUserId());

            return Ok(ApiResponse.Success($"Đã cập nhật trạng thái đơn hàng thành {newStatus}"));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponse.Fail(ex.Message));
        }
    }

    /// <summary>
    /// [Admin] Thống kê đơn hàng
    /// </summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(OrderStatsResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<OrderStatsResponse>> GetOrderStats()
    {
        var orders = await _orderRepository.GetAllAsync();
        var orderList = orders.ToList();

        var completedOrders = orderList.Where(o => o.Status == OrderStatus.Completed || o.Status == OrderStatus.Delivered);

        return Ok(new OrderStatsResponse
        {
            TotalOrders = orderList.Count,
            PendingOrders = orderList.Count(o => o.Status == OrderStatus.Pending),
            ConfirmedOrders = orderList.Count(o => o.Status == OrderStatus.Confirmed),
            ProcessingOrders = orderList.Count(o => o.Status == OrderStatus.Processing),
            ShippingOrders = orderList.Count(o => o.Status == OrderStatus.Shipping),
            DeliveredOrders = orderList.Count(o => o.Status == OrderStatus.Delivered),
            CompletedOrders = orderList.Count(o => o.Status == OrderStatus.Completed),
            CancelledOrders = orderList.Count(o => o.Status == OrderStatus.Cancelled),
            TotalRevenue = completedOrders.Sum(o => o.TotalAmount),
            TodayRevenue = completedOrders
                .Where(o => o.CreatedAt.Date == DateTime.UtcNow.Date)
                .Sum(o => o.TotalAmount),
            ThisMonthRevenue = completedOrders
                .Where(o => o.CreatedAt.Month == DateTime.UtcNow.Month && o.CreatedAt.Year == DateTime.UtcNow.Year)
                .Sum(o => o.TotalAmount),
            OrdersByPaymentMethod = orderList
                .GroupBy(o => o.PaymentMethod.ToString())
                .ToDictionary(g => g.Key, g => g.Count())
        });
    }

    /// <summary>
    /// [Admin/Staff] Lấy đơn hàng theo mã đơn
    /// </summary>
    [HttpGet("by-number/{orderNumber}")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetByOrderNumber(string orderNumber)
    {
        var order = await _orderRepository.GetByOrderNumberAsync(orderNumber);
        if (order == null)
        {
            return NotFound(ApiResponse.Fail("Đơn hàng không tồn tại"));
        }

        // Load items
        var orderWithItems = await _orderRepository.GetWithItemsAsync(order.Id);
        return Ok(MapToOrderResponse(orderWithItems!));
    }

    /// <summary>
    /// [Admin/Staff] Lấy đơn hàng chờ xử lý
    /// </summary>
    [HttpGet("pending")]
    [Authorize(Roles = "Admin,Staff")]
    [ProducesResponseType(typeof(IEnumerable<OrderSummaryResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrderSummaryResponse>>> GetPendingOrders()
    {
        var orders = await _orderRepository.GetPendingOrdersAsync();
        return Ok(orders.Select(MapToOrderSummary));
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

    private static OrderResponse MapToOrderResponse(Order order)
    {
        return new OrderResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            UserId = order.UserId,
            UserName = order.User?.FullName,
            UserEmail = order.User?.Email,
            SubTotal = order.SubTotal,
            ShippingFee = order.ShippingFee,
            TotalDiscount = order.TotalDiscount,
            TotalAmount = order.TotalAmount,
            Status = order.Status.ToString(),
            StatusDescription = order.Status.GetDescription(),
            PaymentMethod = order.PaymentMethod.ToString(),
            PaymentTransactionId = order.PaymentTransactionId,
            PaidAt = order.PaidAt,
            ShippingAddress = order.ShippingAddress,
            ShippingPhone = order.ShippingPhone,
            ReceiverName = order.ReceiverName,
            Notes = order.Notes,
            CouponCode = order.CouponCode,
            CreatedAt = order.CreatedAt,
            EstimatedDeliveryDate = order.EstimatedDeliveryDate,
            DeliveredAt = order.DeliveredAt,
            CancellationReason = order.CancellationReason,
            CancelledAt = order.CancelledAt,
            Items = order.OrderItems.Select(MapToOrderItemResponse).ToList()
        };
    }

    private static OrderItemResponse MapToOrderItemResponse(OrderItem item)
    {
        return new OrderItemResponse
        {
            Id = item.Id,
            ProductId = item.ProductId,
            ProductName = item.ProductName,
            ProductImageUrl = item.Product?.ImageUrl,
            UnitPrice = item.UnitPrice,
            DiscountedPrice = item.DiscountedPrice,
            Quantity = item.Quantity,
            TotalPrice = item.TotalPrice,
            DiscountDescription = item.DiscountDescription
        };
    }

    private static OrderSummaryResponse MapToOrderSummary(Order order)
    {
        return new OrderSummaryResponse
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            Status = order.Status.ToString(),
            StatusDescription = order.Status.GetDescription(),
            TotalAmount = order.TotalAmount,
            ItemCount = order.OrderItems.Count,
            PaymentMethod = order.PaymentMethod.ToString(),
            CreatedAt = order.CreatedAt
        };
    }

    #endregion
}

#region Additional Response Models

public record PaymentResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public int OrderId { get; init; }
    public string? TransactionId { get; init; }
    public string? RedirectUrl { get; init; }
    public string? QrCodeData { get; init; }
}

#endregion

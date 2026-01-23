using System.ComponentModel.DataAnnotations;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.API.ViewModels;

#region Request Models

/// <summary>
/// Request tạo đơn hàng mới
/// </summary>
public record CreateOrderRequest
{
    [Required(ErrorMessage = "Danh sách sản phẩm là bắt buộc")]
    [MinLength(1, ErrorMessage = "Đơn hàng phải có ít nhất 1 sản phẩm")]
    public List<OrderItemRequest> Items { get; init; } = new();

    [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
    [MaxLength(500)]
    public string ShippingAddress { get; init; } = string.Empty;

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string ShippingPhone { get; init; } = string.Empty;

    [Required(ErrorMessage = "Tên người nhận là bắt buộc")]
    [MaxLength(100)]
    public string ReceiverName { get; init; } = string.Empty;

    [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc")]
    public PaymentMethod PaymentMethod { get; init; }

    public string? Notes { get; init; }

    public string? CouponCode { get; init; }
}

/// <summary>
/// Chi tiết sản phẩm trong đơn hàng
/// </summary>
public record OrderItemRequest
{
    [Required]
    public int ProductId { get; init; }

    [Required]
    [Range(1, 100, ErrorMessage = "Số lượng phải từ 1-100")]
    public int Quantity { get; init; }
}

/// <summary>
/// Request hủy đơn hàng
/// </summary>
public record CancelOrderRequest
{
    [Required(ErrorMessage = "Lý do hủy là bắt buộc")]
    [MaxLength(500)]
    public string Reason { get; init; } = string.Empty;
}

/// <summary>
/// Request xác nhận đơn hàng (Admin/Staff)
/// </summary>
public record ConfirmOrderRequest
{
    [Range(0, 1000000, ErrorMessage = "Phí ship không hợp lệ")]
    public decimal ShippingFee { get; init; } = 30000; // Mặc định 30k
}

/// <summary>
/// Request thanh toán
/// </summary>
public record PayOrderRequest
{
    [Required]
    public PaymentMethod PaymentMethod { get; init; }

    /// <summary>
    /// URL callback sau khi thanh toán (cho MoMo, VNPay...)
    /// </summary>
    public string? ReturnUrl { get; init; }
}

#endregion

#region Response Models

/// <summary>
/// Response thông tin đơn hàng
/// </summary>
public record OrderResponse
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public int UserId { get; init; }
    public string? UserName { get; init; }
    public string? UserEmail { get; init; }

    // Thông tin tài chính
    public decimal SubTotal { get; init; }
    public decimal ShippingFee { get; init; }
    public decimal TotalDiscount { get; init; }
    public decimal TotalAmount { get; init; }

    // Trạng thái
    public string Status { get; init; } = string.Empty;
    public string StatusDescription { get; init; } = string.Empty;

    // Thanh toán
    public string PaymentMethod { get; init; } = string.Empty;
    public string? PaymentTransactionId { get; init; }
    public DateTime? PaidAt { get; init; }

    // Thông tin giao hàng
    public string ShippingAddress { get; init; } = string.Empty;
    public string ShippingPhone { get; init; } = string.Empty;
    public string ReceiverName { get; init; } = string.Empty;
    public string? Notes { get; init; }
    public string? CouponCode { get; init; }

    // Thời gian
    public DateTime CreatedAt { get; init; }
    public DateTime? EstimatedDeliveryDate { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public string? CancellationReason { get; init; }
    public DateTime? CancelledAt { get; init; }

    // Danh sách sản phẩm
    public List<OrderItemResponse> Items { get; init; } = new();
}

/// <summary>
/// Response chi tiết sản phẩm trong đơn hàng
/// </summary>
public record OrderItemResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string? ProductImageUrl { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountedPrice { get; init; }
    public int Quantity { get; init; }
    public decimal TotalPrice { get; init; }
    public string? DiscountDescription { get; init; }
}

/// <summary>
/// Response tạo đơn hàng
/// </summary>
public record CreateOrderResponse
{
    public bool IsSuccess { get; init; }
    public string Message { get; init; } = string.Empty;
    public int? OrderId { get; init; }
    public string? OrderNumber { get; init; }
    public decimal? TotalAmount { get; init; }
    public string? PaymentUrl { get; init; } // URL thanh toán (nếu cần redirect)
}

/// <summary>
/// Response danh sách đơn hàng (phân trang)
/// </summary>
public record OrderListResponse
{
    public List<OrderSummaryResponse> Orders { get; init; } = new();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages { get; init; }
}

/// <summary>
/// Tóm tắt đơn hàng (cho danh sách)
/// </summary>
public record OrderSummaryResponse
{
    public int Id { get; init; }
    public string OrderNumber { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string StatusDescription { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public int ItemCount { get; init; }
    public string PaymentMethod { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

/// <summary>
/// Thống kê đơn hàng
/// </summary>
public record OrderStatsResponse
{
    public int TotalOrders { get; init; }
    public int PendingOrders { get; init; }
    public int ConfirmedOrders { get; init; }
    public int ProcessingOrders { get; init; }
    public int ShippingOrders { get; init; }
    public int DeliveredOrders { get; init; }
    public int CompletedOrders { get; init; }
    public int CancelledOrders { get; init; }
    public decimal TotalRevenue { get; init; }
    public decimal TodayRevenue { get; init; }
    public decimal ThisMonthRevenue { get; init; }
    public Dictionary<string, int> OrdersByPaymentMethod { get; init; } = new();
}

#endregion


using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Commands.Orders;

/// <summary>
/// Command tạo đơn hàng mới
/// 
/// COMMAND PATTERN:
/// - Đóng gói tất cả dữ liệu cần thiết để tạo Order
/// - Không chứa logic xử lý
/// - Được truyền đến CreateOrderCommandHandler
/// 
/// Input:
/// - UserId: ID khách hàng
/// - Items: Danh sách sản phẩm (ProductId, Quantity)
/// - ShippingAddress, Phone, ReceiverName
/// - PaymentMethod
/// - CouponCode (optional)
/// 
/// Output: OrderId của đơn hàng mới
/// </summary>
public class CreateOrderCommand : CommandBase<CreateOrderResult>
{
    /// <summary>
    /// ID khách hàng
    /// </summary>
    public int UserId { get; }

    /// <summary>
    /// Danh sách sản phẩm trong đơn hàng
    /// </summary>
    public IReadOnlyList<OrderItemInput> Items { get; }

    /// <summary>
    /// Địa chỉ giao hàng
    /// </summary>
    public string ShippingAddress { get; }

    /// <summary>
    /// Số điện thoại nhận hàng
    /// </summary>
    public string ShippingPhone { get; }

    /// <summary>
    /// Tên người nhận
    /// </summary>
    public string ReceiverName { get; }

    /// <summary>
    /// Phương thức thanh toán
    /// </summary>
    public PaymentMethod PaymentMethod { get; }

    /// <summary>
    /// Ghi chú đơn hàng
    /// </summary>
    public string? Notes { get; }

    /// <summary>
    /// Mã giảm giá
    /// </summary>
    public string? CouponCode { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public CreateOrderCommand(
        int userId,
        IEnumerable<OrderItemInput> items,
        string shippingAddress,
        string shippingPhone,
        string receiverName,
        PaymentMethod paymentMethod,
        string? notes = null,
        string? couponCode = null)
    {
        UserId = userId;
        Items = items.ToList().AsReadOnly();
        ShippingAddress = shippingAddress;
        ShippingPhone = shippingPhone;
        ReceiverName = receiverName;
        PaymentMethod = paymentMethod;
        Notes = notes;
        CouponCode = couponCode;
    }
}

/// <summary>
/// Input cho mỗi item trong đơn hàng
/// </summary>
public class OrderItemInput
{
    /// <summary>
    /// ID sản phẩm
    /// </summary>
    public int ProductId { get; set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public int Quantity { get; set; }
}

/// <summary>
/// Kết quả tạo đơn hàng
/// </summary>
public class CreateOrderResult
{
    /// <summary>
    /// ID đơn hàng mới
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tổng tiền
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Trạng thái
    /// </summary>
    public OrderStatus Status { get; set; }

    /// <summary>
    /// URL thanh toán (nếu thanh toán online)
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string Message { get; set; } = string.Empty;
}


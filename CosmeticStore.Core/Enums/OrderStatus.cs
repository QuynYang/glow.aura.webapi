namespace CosmeticStore.Core.Enums;

/// <summary>
/// Enum trạng thái đơn hàng - Dùng trong Command Pattern
/// Mỗi Command sẽ thay đổi trạng thái theo workflow
/// </summary>
public enum OrderStatus
{
    /// <summary>
    /// Đơn hàng mới tạo, chờ xác nhận
    /// </summary>
    Pending = 0,

    /// <summary>
    /// Đã xác nhận, chờ thanh toán
    /// </summary>
    Confirmed = 1,

    /// <summary>
    /// Đã thanh toán, chờ xử lý
    /// </summary>
    Paid = 2,

    /// <summary>
    /// Đang xử lý/đóng gói
    /// </summary>
    Processing = 3,

    /// <summary>
    /// Đang giao hàng
    /// </summary>
    Shipping = 4,

    /// <summary>
    /// Đã giao hàng thành công
    /// </summary>
    Delivered = 5,

    /// <summary>
    /// Hoàn thành (sau khi khách nhận hàng)
    /// </summary>
    Completed = 6,

    /// <summary>
    /// Đã hủy
    /// </summary>
    Cancelled = 7,

    /// <summary>
    /// Đã hoàn tiền
    /// </summary>
    Refunded = 8,

    /// <summary>
    /// Thanh toán thất bại
    /// </summary>
    PaymentFailed = 9
}

/// <summary>
/// Extension methods cho OrderStatus
/// </summary>
public static class OrderStatusExtensions
{
    /// <summary>
    /// Kiểm tra xem có thể hủy đơn hàng không
    /// </summary>
    public static bool CanCancel(this OrderStatus status)
    {
        return status is OrderStatus.Pending or OrderStatus.Confirmed or OrderStatus.PaymentFailed;
    }

    /// <summary>
    /// Kiểm tra xem có thể xác nhận đơn hàng không
    /// </summary>
    public static bool CanConfirm(this OrderStatus status)
    {
        return status == OrderStatus.Pending;
    }

    /// <summary>
    /// Kiểm tra xem có thể thanh toán không
    /// </summary>
    public static bool CanPay(this OrderStatus status)
    {
        return status is OrderStatus.Confirmed or OrderStatus.PaymentFailed;
    }

    /// <summary>
    /// Kiểm tra xem đơn hàng đã hoàn thành chưa
    /// </summary>
    public static bool IsFinal(this OrderStatus status)
    {
        return status is OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.Refunded;
    }

    /// <summary>
    /// Lấy mô tả tiếng Việt
    /// </summary>
    public static string GetDescription(this OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => "Chờ xác nhận",
            OrderStatus.Confirmed => "Đã xác nhận",
            OrderStatus.Paid => "Đã thanh toán",
            OrderStatus.Processing => "Đang xử lý",
            OrderStatus.Shipping => "Đang giao hàng",
            OrderStatus.Delivered => "Đã giao hàng",
            OrderStatus.Completed => "Hoàn thành",
            OrderStatus.Cancelled => "Đã hủy",
            OrderStatus.Refunded => "Đã hoàn tiền",
            OrderStatus.PaymentFailed => "Thanh toán thất bại",
            _ => "Không xác định"
        };
    }
}


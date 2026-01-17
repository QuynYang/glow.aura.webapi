namespace CosmeticStore.Core.Commands.Orders;

/// <summary>
/// Command hủy đơn hàng
/// 
/// COMMAND PATTERN:
/// - Đóng gói OrderId và Reason
/// - Cho phép hoàn tác (undo) nếu cần
/// - Dễ dàng log và audit
/// 
/// Business Rules:
/// - Chỉ hủy được đơn hàng ở trạng thái Pending, Confirmed, PaymentFailed
/// - Phải có lý do hủy
/// - Nếu đã thanh toán -> cần hoàn tiền
/// </summary>
public class CancelOrderCommand : CommandBase<CancelOrderResult>
{
    /// <summary>
    /// ID đơn hàng cần hủy
    /// </summary>
    public int OrderId { get; }

    /// <summary>
    /// Lý do hủy đơn
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// ID người hủy (User hoặc Admin)
    /// </summary>
    public int CancelledByUserId { get; }

    /// <summary>
    /// Có phải Admin hủy không
    /// </summary>
    public bool IsCancelledByAdmin { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public CancelOrderCommand(
        int orderId,
        string reason,
        int cancelledByUserId,
        bool isCancelledByAdmin = false)
    {
        OrderId = orderId;
        Reason = reason;
        CancelledByUserId = cancelledByUserId;
        IsCancelledByAdmin = isCancelledByAdmin;
    }
}

/// <summary>
/// Kết quả hủy đơn hàng
/// </summary>
public class CancelOrderResult
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Có cần hoàn tiền không
    /// </summary>
    public bool RequiresRefund { get; set; }

    /// <summary>
    /// Số tiền cần hoàn
    /// </summary>
    public decimal RefundAmount { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian hủy
    /// </summary>
    public DateTime CancelledAt { get; set; }
}


namespace CosmeticStore.Core.Commands.Orders;

/// <summary>
/// Command xác nhận đơn hàng
/// 
/// COMMAND PATTERN:
/// - Chuyển trạng thái từ Pending -> Confirmed
/// - Có thể kèm phí ship và giảm giá
/// 
/// Business Rules:
/// - Chỉ xác nhận được đơn hàng Pending
/// - Phải có ít nhất 1 sản phẩm
/// - Tất cả sản phẩm phải còn hàng
/// </summary>
public class ConfirmOrderCommand : CommandBase<ConfirmOrderResult>
{
    /// <summary>
    /// ID đơn hàng cần xác nhận
    /// </summary>
    public int OrderId { get; }

    /// <summary>
    /// ID Admin xác nhận
    /// </summary>
    public int ConfirmedByAdminId { get; }

    /// <summary>
    /// Phí vận chuyển (nếu có thay đổi)
    /// </summary>
    public decimal? ShippingFee { get; }

    /// <summary>
    /// Ghi chú của Admin
    /// </summary>
    public string? AdminNotes { get; }

    /// <summary>
    /// Ngày giao hàng dự kiến
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; }

    /// <summary>
    /// Constructor
    /// </summary>
    public ConfirmOrderCommand(
        int orderId,
        int confirmedByAdminId,
        decimal? shippingFee = null,
        string? adminNotes = null,
        DateTime? estimatedDeliveryDate = null)
    {
        OrderId = orderId;
        ConfirmedByAdminId = confirmedByAdminId;
        ShippingFee = shippingFee;
        AdminNotes = adminNotes;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }
}

/// <summary>
/// Kết quả xác nhận đơn hàng
/// </summary>
public class ConfirmOrderResult
{
    /// <summary>
    /// Mã đơn hàng
    /// </summary>
    public string OrderNumber { get; set; } = string.Empty;

    /// <summary>
    /// Tổng tiền sau khi xác nhận
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Phí ship
    /// </summary>
    public decimal ShippingFee { get; set; }

    /// <summary>
    /// Ngày giao dự kiến
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; set; }

    /// <summary>
    /// Cần thanh toán online không
    /// </summary>
    public bool RequiresOnlinePayment { get; set; }

    /// <summary>
    /// URL thanh toán (nếu có)
    /// </summary>
    public string? PaymentUrl { get; set; }

    /// <summary>
    /// Thông báo
    /// </summary>
    public string Message { get; set; } = string.Empty;
}


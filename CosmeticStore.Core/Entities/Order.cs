using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Entities;

/// <summary>
/// Entity Đơn hàng - Aggregate Root
/// 
/// OOP - ENCAPSULATION:
/// - Tất cả logic nghiệp vụ nằm trong class
/// - Không cho phép thay đổi trực tiếp từ bên ngoài
/// 
/// COMMAND PATTERN:
/// - Mỗi thay đổi trạng thái được thực hiện qua Command
/// - CreateOrderCommand -> tạo Order
/// - ConfirmOrderCommand -> Confirm()
/// - CancelOrderCommand -> Cancel()
/// </summary>
public class Order : BaseEntity
{
    private readonly List<OrderItem> _orderItems = new();

    /// <summary>
    /// Mã đơn hàng (hiển thị cho khách)
    /// </summary>
    public string OrderNumber { get; private set; } = string.Empty;

    /// <summary>
    /// ID khách hàng (Foreign Key)
    /// </summary>
    public int UserId { get; private set; }

    /// <summary>
    /// Trạng thái đơn hàng
    /// </summary>
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    /// <summary>
    /// Tổng tiền hàng (chưa ship)
    /// </summary>
    public decimal SubTotal { get; private set; }

    /// <summary>
    /// Phí vận chuyển
    /// </summary>
    public decimal ShippingFee { get; private set; }

    /// <summary>
    /// Tổng giảm giá
    /// </summary>
    public decimal TotalDiscount { get; private set; }

    /// <summary>
    /// Tổng thanh toán = SubTotal + ShippingFee - TotalDiscount
    /// </summary>
    public decimal TotalAmount => SubTotal + ShippingFee - TotalDiscount;

    /// <summary>
    /// Phương thức thanh toán
    /// </summary>
    public PaymentMethod PaymentMethod { get; private set; }

    /// <summary>
    /// Mã giao dịch thanh toán (từ cổng thanh toán)
    /// </summary>
    public string? PaymentTransactionId { get; private set; }

    /// <summary>
    /// Thời gian thanh toán
    /// </summary>
    public DateTime? PaidAt { get; private set; }

    /// <summary>
    /// Mã coupon đã dùng
    /// </summary>
    public string? CouponCode { get; private set; }

    /// <summary>
    /// Địa chỉ giao hàng
    /// </summary>
    public string ShippingAddress { get; private set; } = string.Empty;

    /// <summary>
    /// Số điện thoại nhận hàng
    /// </summary>
    public string ShippingPhone { get; private set; } = string.Empty;

    /// <summary>
    /// Tên người nhận
    /// </summary>
    public string ReceiverName { get; private set; } = string.Empty;

    /// <summary>
    /// Ghi chú đơn hàng
    /// </summary>
    public string? Notes { get; private set; }

    /// <summary>
    /// Lý do hủy đơn (nếu có)
    /// </summary>
    public string? CancellationReason { get; private set; }

    /// <summary>
    /// Thời gian hủy
    /// </summary>
    public DateTime? CancelledAt { get; private set; }

    /// <summary>
    /// Thời gian giao hàng dự kiến
    /// </summary>
    public DateTime? EstimatedDeliveryDate { get; private set; }

    /// <summary>
    /// Thời gian giao hàng thực tế
    /// </summary>
    public DateTime? DeliveredAt { get; private set; }

    /// <summary>
    /// Navigation property - User
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Danh sách sản phẩm trong đơn hàng (Read-only)
    /// </summary>
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected Order() { }

    /// <summary>
    /// Constructor tạo Order mới
    /// </summary>
    public Order(
        int userId,
        string shippingAddress,
        string shippingPhone,
        string receiverName,
        PaymentMethod paymentMethod,
        string? notes = null,
        string? couponCode = null)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            throw new ArgumentException("Địa chỉ giao hàng không được để trống", nameof(shippingAddress));
        if (string.IsNullOrWhiteSpace(shippingPhone))
            throw new ArgumentException("Số điện thoại không được để trống", nameof(shippingPhone));
        if (string.IsNullOrWhiteSpace(receiverName))
            throw new ArgumentException("Tên người nhận không được để trống", nameof(receiverName));

        UserId = userId;
        OrderNumber = GenerateOrderNumber();
        ShippingAddress = shippingAddress;
        ShippingPhone = shippingPhone;
        ReceiverName = receiverName;
        PaymentMethod = paymentMethod;
        Notes = notes;
        CouponCode = couponCode;
        Status = OrderStatus.Pending;
    }

    #region Domain Methods (Encapsulation)

    /// <summary>
    /// Thêm sản phẩm vào đơn hàng
    /// </summary>
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Không thể thêm sản phẩm vào đơn hàng đã xác nhận");

        item.SetOrderId(this.Id);
        _orderItems.Add(item);
        RecalculateTotals();
    }

    /// <summary>
    /// Xóa sản phẩm khỏi đơn hàng
    /// </summary>
    public void RemoveItem(int productId)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Không thể xóa sản phẩm khỏi đơn hàng đã xác nhận");

        var item = _orderItems.FirstOrDefault(x => x.ProductId == productId);
        if (item != null)
        {
            _orderItems.Remove(item);
            RecalculateTotals();
        }
    }

    /// <summary>
    /// Xác nhận đơn hàng - Gọi từ ConfirmOrderCommand
    /// </summary>
    public void Confirm()
    {
        if (!Status.CanConfirm())
            throw new InvalidOperationException($"Không thể xác nhận đơn hàng ở trạng thái {Status.GetDescription()}");

        if (!_orderItems.Any())
            throw new InvalidOperationException("Đơn hàng không có sản phẩm");

        Status = OrderStatus.Confirmed;
        EstimatedDeliveryDate = DateTime.UtcNow.AddDays(3); // Dự kiến 3 ngày
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hủy đơn hàng - Gọi từ CancelOrderCommand
    /// </summary>
    public void Cancel(string reason)
    {
        if (!Status.CanCancel())
            throw new InvalidOperationException($"Không thể hủy đơn hàng ở trạng thái {Status.GetDescription()}");

        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hủy đơn", nameof(reason));

        Status = OrderStatus.Cancelled;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đánh dấu đã thanh toán
    /// </summary>
    public void MarkAsPaid(string transactionId)
    {
        if (!Status.CanPay())
            throw new InvalidOperationException($"Không thể thanh toán đơn hàng ở trạng thái {Status.GetDescription()}");

        Status = OrderStatus.Paid;
        PaymentTransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đánh dấu thanh toán thất bại
    /// </summary>
    public void MarkPaymentFailed(string? reason = null)
    {
        Status = OrderStatus.PaymentFailed;
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\n{reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Chuyển sang trạng thái đang xử lý
    /// </summary>
    public void StartProcessing()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Đơn hàng chưa thanh toán");

        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Chuyển sang trạng thái đang giao
    /// </summary>
    public void StartShipping()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Đơn hàng chưa được xử lý");

        Status = OrderStatus.Shipping;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Đánh dấu đã giao hàng
    /// </summary>
    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipping)
            throw new InvalidOperationException("Đơn hàng chưa được giao");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hoàn thành đơn hàng
    /// </summary>
    public void Complete()
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Đơn hàng chưa được giao");

        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hoàn tiền
    /// </summary>
    public void Refund(string reason)
    {
        if (Status != OrderStatus.Paid && Status != OrderStatus.Processing)
            throw new InvalidOperationException("Chỉ có thể hoàn tiền đơn hàng đã thanh toán");

        Status = OrderStatus.Refunded;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật phí ship
    /// </summary>
    public void SetShippingFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentException("Phí ship không được âm", nameof(fee));

        ShippingFee = fee;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Áp dụng giảm giá tổng
    /// </summary>
    public void ApplyDiscount(decimal discount)
    {
        if (discount < 0)
            throw new ArgumentException("Giảm giá không được âm", nameof(discount));
        if (discount > SubTotal)
            throw new ArgumentException("Giảm giá không được lớn hơn tổng tiền hàng", nameof(discount));

        TotalDiscount = discount;
        UpdatedAt = DateTime.UtcNow;
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Tính lại tổng tiền
    /// </summary>
    private void RecalculateTotals()
    {
        SubTotal = _orderItems.Sum(x => x.TotalPrice);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Tạo mã đơn hàng
    /// </summary>
    private static string GenerateOrderNumber()
    {
        // Format: ORD + YYYYMMDD + Random 4 số
        return $"ORD{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    #endregion
}


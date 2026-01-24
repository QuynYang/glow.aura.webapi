using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Entities;

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
public class Order : BaseEntity
{
    private readonly List<OrderItem> _orderItems = new();

    
    /// Mã đơn hàng (hiển thị cho khách)
    
    public string OrderNumber { get; private set; } = string.Empty;

    
    /// ID khách hàng (Foreign Key)
    
    public int UserId { get; private set; }

    
    /// Trạng thái đơn hàng
    
    public OrderStatus Status { get; private set; } = OrderStatus.Pending;

    
    /// Tổng tiền hàng (chưa ship)
    
    public decimal SubTotal { get; private set; }

    
    /// Phí vận chuyển
    
    public decimal ShippingFee { get; private set; }

    
    /// Tổng giảm giá
    
    public decimal TotalDiscount { get; private set; }

    
    /// Tổng thanh toán = SubTotal + ShippingFee - TotalDiscount
    
    public decimal TotalAmount => SubTotal + ShippingFee - TotalDiscount;

    
    /// Phương thức thanh toán
    
    public PaymentMethod PaymentMethod { get; private set; }

    
    /// Mã giao dịch thanh toán (từ cổng thanh toán)
    
    public string? PaymentTransactionId { get; private set; }

    
    /// Thời gian thanh toán
    
    public DateTime? PaidAt { get; private set; }

    
    /// Mã coupon đã dùng
    
    public string? CouponCode { get; private set; }

    
    /// Địa chỉ giao hàng
    
    public string ShippingAddress { get; private set; } = string.Empty;

    
    /// Số điện thoại nhận hàng
    
    public string ShippingPhone { get; private set; } = string.Empty;

    
    /// Tên người nhận
    
    public string ReceiverName { get; private set; } = string.Empty;

    
    /// Ghi chú đơn hàng
    
    public string? Notes { get; private set; }

    
    /// Lý do hủy đơn (nếu có)
    
    public string? CancellationReason { get; private set; }

    
    /// Thời gian hủy
    
    public DateTime? CancelledAt { get; private set; }

    
    /// Thời gian giao hàng dự kiến
    
    public DateTime? EstimatedDeliveryDate { get; private set; }

    
    /// Thời gian giao hàng thực tế
    
    public DateTime? DeliveredAt { get; private set; }

    
    /// Navigation property - User
    
    public User User { get; private set; } = null!;

    
    /// Danh sách sản phẩm trong đơn hàng (Read-only)
    
    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    
    /// Constructor mặc định cho EF Core
    
    protected Order() { }

    
    /// Constructor tạo Order mới
    
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

    
    /// Thêm sản phẩm vào đơn hàng
    
    public void AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            throw new InvalidOperationException("Không thể thêm sản phẩm vào đơn hàng đã xác nhận");

        item.SetOrderId(this.Id);
        _orderItems.Add(item);
        RecalculateTotals();
    }

    
    /// Xóa sản phẩm khỏi đơn hàng
    
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

    
    /// Xác nhận đơn hàng - Gọi từ ConfirmOrderCommand
    
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

    
    /// Hủy đơn hàng - Gọi từ CancelOrderCommand
    
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

    
    /// Đánh dấu đã thanh toán
    
    public void MarkAsPaid(string transactionId)
    {
        if (!Status.CanPay())
            throw new InvalidOperationException($"Không thể thanh toán đơn hàng ở trạng thái {Status.GetDescription()}");

        Status = OrderStatus.Paid;
        PaymentTransactionId = transactionId;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Đánh dấu thanh toán thất bại
    
    public void MarkPaymentFailed(string? reason = null)
    {
        Status = OrderStatus.PaymentFailed;
        Notes = string.IsNullOrEmpty(Notes) ? reason : $"{Notes}\n{reason}";
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Chuyển sang trạng thái đang xử lý
    
    public void StartProcessing()
    {
        if (Status != OrderStatus.Paid)
            throw new InvalidOperationException("Đơn hàng chưa thanh toán");

        Status = OrderStatus.Processing;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Chuyển sang trạng thái đang giao
    
    public void StartShipping()
    {
        if (Status != OrderStatus.Processing)
            throw new InvalidOperationException("Đơn hàng chưa được xử lý");

        Status = OrderStatus.Shipping;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Đánh dấu đã giao hàng
    
    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipping)
            throw new InvalidOperationException("Đơn hàng chưa được giao");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Hoàn thành đơn hàng
    
    public void Complete()
    {
        if (Status != OrderStatus.Delivered)
            throw new InvalidOperationException("Đơn hàng chưa được giao");

        Status = OrderStatus.Completed;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Hoàn tiền
    
    public void Refund(string reason)
    {
        if (Status != OrderStatus.Paid && Status != OrderStatus.Processing)
            throw new InvalidOperationException("Chỉ có thể hoàn tiền đơn hàng đã thanh toán");

        Status = OrderStatus.Refunded;
        CancellationReason = reason;
        CancelledAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Cập nhật phí ship
    
    public void SetShippingFee(decimal fee)
    {
        if (fee < 0)
            throw new ArgumentException("Phí ship không được âm", nameof(fee));

        ShippingFee = fee;
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Áp dụng giảm giá tổng
    
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

    
    /// Tính lại tổng tiền
    
    private void RecalculateTotals()
    {
        SubTotal = _orderItems.Sum(x => x.TotalPrice);
        UpdatedAt = DateTime.UtcNow;
    }

    
    /// Tạo mã đơn hàng
    
    private static string GenerateOrderNumber()
    {
        // Format: ORD + YYYYMMDD + Random 4 số
        return $"ORD{DateTime.UtcNow:yyyyMMdd}{Random.Shared.Next(1000, 9999)}";
    }

    #endregion
}


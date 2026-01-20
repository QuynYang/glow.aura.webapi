using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Events;

/// <summary>
/// Event khi đơn hàng được tạo
/// 
/// OBSERVER PATTERN:
/// - Raised by: CreateOrderCommandHandler
/// - Handled by: EmailNotificationHandler, SmsNotificationHandler, AppNotificationHandler
/// </summary>
public class OrderCreatedEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public string UserPhone { get; }
    public string UserName { get; }
    public decimal TotalAmount { get; }
    public int ItemCount { get; }
    public string ShippingAddress { get; }
    public PaymentMethod PaymentMethod { get; }

    public OrderCreatedEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        string userPhone,
        string userName,
        decimal totalAmount,
        int itemCount,
        string shippingAddress,
        PaymentMethod paymentMethod)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        UserPhone = userPhone;
        UserName = userName;
        TotalAmount = totalAmount;
        ItemCount = itemCount;
        ShippingAddress = shippingAddress;
        PaymentMethod = paymentMethod;
    }
}

/// <summary>
/// Event khi đơn hàng được xác nhận
/// </summary>
public class OrderConfirmedEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public string UserPhone { get; }
    public DateTime EstimatedDeliveryDate { get; }

    public OrderConfirmedEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        string userPhone,
        DateTime estimatedDeliveryDate)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        UserPhone = userPhone;
        EstimatedDeliveryDate = estimatedDeliveryDate;
    }
}

/// <summary>
/// Event khi đơn hàng bị hủy
/// </summary>
public class OrderCancelledEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public string CancellationReason { get; }
    public decimal RefundAmount { get; }

    public OrderCancelledEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        string cancellationReason,
        decimal refundAmount)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        CancellationReason = cancellationReason;
        RefundAmount = refundAmount;
    }
}

/// <summary>
/// Event khi thanh toán thành công
/// </summary>
public class PaymentSuccessEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public string PaymentMethod { get; }
    public decimal Amount { get; }
    public string TransactionId { get; }

    public PaymentSuccessEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        string paymentMethod,
        decimal amount,
        string transactionId)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        PaymentMethod = paymentMethod;
        Amount = amount;
        TransactionId = transactionId;
    }
}

/// <summary>
/// Event khi thanh toán thất bại
/// </summary>
public class PaymentFailedEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public string PaymentMethod { get; }
    public string ErrorMessage { get; }

    public PaymentFailedEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        string paymentMethod,
        string errorMessage)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        PaymentMethod = paymentMethod;
        ErrorMessage = errorMessage;
    }
}

/// <summary>
/// Event khi đơn hàng được giao thành công
/// </summary>
public class OrderDeliveredEvent : DomainEventBase
{
    public int OrderId { get; }
    public string OrderNumber { get; }
    public int UserId { get; }
    public string UserEmail { get; }
    public DateTime DeliveredAt { get; }

    public OrderDeliveredEvent(
        int orderId,
        string orderNumber,
        int userId,
        string userEmail,
        DateTime deliveredAt)
    {
        OrderId = orderId;
        OrderNumber = orderNumber;
        UserId = userId;
        UserEmail = userEmail;
        DeliveredAt = deliveredAt;
    }
}


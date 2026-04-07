using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// Factory chuyển đổi OrderStatus enum → IOrderState object
/// VÍ DỤ:
/// var state = OrderStateFactory.Create(OrderStatus.Pending); // → PendingState object
/// var newState = state.Confirm();                            // → ConfirmedState object
/// ENCAPSULATION:
/// Đặt tại tầng Core cùng với Order entity để Order có thể
/// tự khởi tạo _currentState mà không phụ thuộc Infrastructure

public static class OrderStateFactory
{
    
    /// Tạo IOrderState từ OrderStatus enum
    /// <param name="status">Trạng thái hiện tại của đơn hàng</param>
    /// <returns>State object tương ứng</returns>
    public static IOrderState Create(OrderStatus status)
    {
        return status switch
        {
            OrderStatus.Pending => new PendingState(),
            OrderStatus.Confirmed => new ConfirmedState(),
            OrderStatus.Paid => new PaidState(),
            OrderStatus.Processing => new ProcessingState(),
            OrderStatus.Shipping => new ShippingState(),
            OrderStatus.Delivered => new DeliveredState(),
            OrderStatus.Completed => new CompletedState(),
            OrderStatus.Cancelled => new CancelledState(),
            OrderStatus.Refunded => new RefundedState(),
            OrderStatus.PaymentFailed => new PaymentFailedState(),
            _ => throw new ArgumentException($"Trạng thái không hợp lệ: {status}")
        };
    }
}

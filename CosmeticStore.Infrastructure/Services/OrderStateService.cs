using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Core.States;

namespace CosmeticStore.Infrastructure.Services;


/// STATE PATTERN - Service điều phối chuyển trạng thái đơn hàng
///   1. Gọi method trên Order (Order tự kiểm tra State bên trong)
///   2. Ghi log chuyển trạng thái
///   3. Cung cấp GetAvailableActions() cho Controller

public interface IOrderStateService
{
    
    /// Xác nhận đơn hàng (Pending → Confirmed)
    
    void ConfirmOrder(Order order);

    
    /// Hủy đơn hàng (Pending/Confirmed/PaymentFailed → Cancelled)
    
    void CancelOrder(Order order, string reason);

    
    /// Thanh toán đơn hàng (Confirmed/PaymentFailed → Paid)
    
    void PayOrder(Order order, string transactionId);

    
    /// Đánh dấu thanh toán thất bại (Confirmed → PaymentFailed)
    
    void MarkPaymentFailed(Order order, string? reason = null);

    
    /// Bắt đầu xử lý (Paid/Confirmed[COD] → Processing)
    
    void StartProcessing(Order order);

    
    /// Bắt đầu giao hàng (Processing → Shipping)
    
    void ShipOrder(Order order);

    
    /// Đánh dấu đã giao (Shipping → Delivered)
    
    void DeliverOrder(Order order);

    
    /// Hoàn thành đơn hàng (Delivered → Completed)
    
    void CompleteOrder(Order order);

    
    /// Hoàn tiền (Paid/Processing → Refunded)
    
    void RefundOrder(Order order, string reason);

    
    /// Lấy danh sách hành động được phép ở trạng thái hiện tại
    
    List<string> GetAvailableActions(OrderStatus currentStatus);
}


/// Implementation của IOrderStateService

/// Sau khi Order entity tự quản lý State Pattern bên trong,
/// Service này trở nên gọn gàng hơn - chỉ gọi Order method + ghi log

public class OrderStateService : IOrderStateService
{
    private readonly IAppLogger _logger;

    public OrderStateService(IAppLogger logger)
    {
        _logger = logger;
    }

    
    /// Xác nhận đơn hàng
    /// Order.Confirm() nội bộ sẽ delegate cho PendingState.Confirm()
    
    public void ConfirmOrder(Order order)
    {
        var oldStatus = order.Status;
        order.Confirm(); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()}");
    }

    
    /// Hủy đơn hàng
    /// Order.Cancel() nội bộ sẽ delegate cho State.Cancel()
    
    public void CancelOrder(Order order, string reason)
    {
        var oldStatus = order.Status;
        order.Cancel(reason); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()} (Lý do: {reason})");
    }

    
    /// Thanh toán đơn hàng
    /// Order.MarkAsPaid() nội bộ sẽ delegate cho State.Pay()
    
    public void PayOrder(Order order, string transactionId)
    {
        var oldStatus = order.Status;
        order.MarkAsPaid(transactionId); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()} (TX: {transactionId})");
    }

    
    /// Đánh dấu thanh toán thất bại
    
    public void MarkPaymentFailed(Order order, string? reason = null)
    {
        var oldStatus = order.Status;
        order.MarkPaymentFailed(reason);

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → Thanh toán thất bại");
    }

    
    /// Bắt đầu xử lý
    /// Order.StartProcessing() nội bộ sẽ delegate cho State.StartProcessing()
    
    public void StartProcessing(Order order)
    {
        var oldStatus = order.Status;
        order.StartProcessing(); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()}");
    }

    
    /// Bắt đầu giao hàng
    /// Order.StartShipping() nội bộ sẽ delegate cho State.Ship()
    
    public void ShipOrder(Order order)
    {
        var oldStatus = order.Status;
        order.StartShipping(); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()}");
    }

    
    /// Đánh dấu đã giao
    /// Order.MarkAsDelivered() nội bộ sẽ delegate cho State.Deliver()
    
    public void DeliverOrder(Order order)
    {
        var oldStatus = order.Status;
        order.MarkAsDelivered(); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()}");
    }

    
    /// Hoàn thành đơn hàng
    /// Order.Complete() nội bộ sẽ delegate cho State.Complete()
    
    public void CompleteOrder(Order order)
    {
        var oldStatus = order.Status;
        order.Complete(); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()}");
    }

    
    /// Hoàn tiền
    /// Order.Refund() nội bộ sẽ delegate cho State.Refund()
    
    public void RefundOrder(Order order, string reason)
    {
        var oldStatus = order.Status;
        order.Refund(reason); // Order tự delegate cho _currentState bên trong

        _logger.LogInfo($"[STATE] Đơn hàng #{order.OrderNumber}: " +
                       $"{oldStatus.GetDescription()} → {order.Status.GetDescription()} (Lý do: {reason})");
    }

    
    /// Lấy danh sách hành động được phép ở trạng thái hiện tại
    /// Sử dụng OrderStateFactory từ Core để tạo State object
    /// Thử từng hành động, nếu không ném lỗi → Được phép
    
    public List<string> GetAvailableActions(OrderStatus currentStatus)
    {
        var state = OrderStateFactory.Create(currentStatus);
        var actions = new List<string>();

        // Thử từng hành động, nếu không ném lỗi → được phép
        if (TryAction(() => state.Confirm())) actions.Add("Confirm");
        if (TryAction(() => state.Cancel("test"))) actions.Add("Cancel");
        if (TryAction(() => state.Pay("test"))) actions.Add("Pay");
        if (TryAction(() => state.StartProcessing())) actions.Add("StartProcessing");
        if (TryAction(() => state.Ship())) actions.Add("Ship");
        if (TryAction(() => state.Deliver())) actions.Add("Deliver");
        if (TryAction(() => state.Complete())) actions.Add("Complete");
        if (TryAction(() => state.Refund("test"))) actions.Add("Refund");

        return actions;
    }

    
    /// Helper: Thử thực hiện action, trả về true nếu không ném lỗi
    
    private static bool TryAction(Func<IOrderState> action)
    {
        try
        {
            action();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

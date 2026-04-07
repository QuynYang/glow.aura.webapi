using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;

/// STATE PATTERN - Lớp cơ sở cho tất cả trạng thái đơn hàng
/// VÍ DỤ:
/// - PendingState override Confirm() và Cancel() → Được phép
/// - PendingState KHÔNG override Pay() → Gọi Pay() sẽ ném lỗi từ base
/// ENCAPSULATION:
/// Đặt tại tầng Core để Order entity có thể sử dụng trực tiếp
/// mà không phụ thuộc vào Infrastructure
public abstract class OrderStateBase : IOrderState
{
    public abstract OrderStatus Status { get; }
    public abstract string Description { get; }


    /// Mặc định: Không cho phép xác nhận

    public virtual IOrderState Confirm()
    {
        throw new InvalidOperationException(
            $"Không thể xác nhận đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép hủy

    public virtual IOrderState Cancel(string reason)
    {
        throw new InvalidOperationException(
            $"Không thể hủy đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép thanh toán

    public virtual IOrderState Pay(string transactionId)
    {
        throw new InvalidOperationException(
            $"Không thể thanh toán đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép xử lý

    public virtual IOrderState StartProcessing()
    {
        throw new InvalidOperationException(
            $"Không thể xử lý đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép giao hàng

    public virtual IOrderState Ship()
    {
        throw new InvalidOperationException(
            $"Không thể giao hàng đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép đánh dấu đã giao

    public virtual IOrderState Deliver()
    {
        throw new InvalidOperationException(
            $"Không thể đánh dấu giao hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép hoàn thành

    public virtual IOrderState Complete()
    {
        throw new InvalidOperationException(
            $"Không thể hoàn thành đơn hàng ở trạng thái '{Description}'");
    }


    /// Mặc định: Không cho phép hoàn tiền

    public virtual IOrderState Refund(string reason)
    {
        throw new InvalidOperationException(
            $"Không thể hoàn tiền đơn hàng ở trạng thái '{Description}'");
    }
}

using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;

/// STATE PATTERN - Trạng thái "Đã giao hàng" (Delivered)
/// Đơn hàng đã giao đến khách, chờ xác nhận hoàn thành.
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Complete() → Chuyển sang CompletedState (khách xác nhận nhận hàng)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Tất cả hành động khác → Đã giao, chỉ chờ hoàn thành
public class DeliveredState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Delivered;
    public override string Description => "Đã giao hàng";

    /// ✅ Hoàn thành đơn hàng → Chuyển sang CompletedState
    public override IOrderState Complete()
    {
        return new CompletedState();
    }
}

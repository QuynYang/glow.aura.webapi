using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;

/// STATE PATTERN - Trạng thái "Đã hoàn tiền" (Refunded)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Tất cả → Đơn hàng đã hoàn tiền, không thể thay đổi
public class RefundedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Refunded;
    public override string Description => "Đã hoàn tiền";

}

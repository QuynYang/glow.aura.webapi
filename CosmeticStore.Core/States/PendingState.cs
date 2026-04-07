using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Chờ xác nhận" (Pending)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Confirm() → Chuyển sang ConfirmedState
/// ✅ Cancel()  → Chuyển sang CancelledState
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Pay()            → Chưa xác nhận, không thể thanh toán
/// ❌ StartProcessing() → Chưa thanh toán
/// ❌ Ship()           → Chưa xử lý
/// ❌ Deliver()        → Chưa giao
/// ❌ Complete()       → Chưa giao xong
/// ❌ Refund()         → Chưa thanh toán, không có gì để hoàn

public class PendingState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Pending;
    public override string Description => "Chờ xác nhận";

    
    /// ✅ Xác nhận đơn hàng → Chuyển sang ConfirmedState
    
    public override IOrderState Confirm()
    {
        return new ConfirmedState();
    }

    
    /// ✅ Hủy đơn hàng → Chuyển sang CancelledState
    
    public override IOrderState Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hủy đơn", nameof(reason));

        return new CancelledState();
    }
}

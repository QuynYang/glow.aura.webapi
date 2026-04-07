using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Đang giao hàng" (Shipping)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Deliver() → Chuyển sang DeliveredState (giao hàng thành công)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Confirm()         → Đã qua bước xác nhận
/// ❌ Cancel()          → Đang giao hàng, không thể hủy
/// ❌ Pay()             → Đã thanh toán
/// ❌ StartProcessing() → Đã qua bước xử lý
/// ❌ Ship()            → Đã đang giao rồi
/// ❌ Complete()        → Chưa giao xong
/// ❌ Refund()          → Đang giao, phải chờ nhận hàng mới hoàn

public class ShippingState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Shipping;
    public override string Description => "Đang giao hàng";

    
    /// ✅ Giao hàng thành công → Chuyển sang DeliveredState
    
    public override IOrderState Deliver()
    {
        return new DeliveredState();
    }
}

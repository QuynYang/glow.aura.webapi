using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Đang xử lý" (Processing)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Ship()   → Chuyển sang ShippingState (bắt đầu giao hàng)
/// ✅ Refund() → Chuyển sang RefundedState (hoàn tiền nếu có vấn đề)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Confirm()        → Đã qua bước xác nhận
/// ❌ Cancel()         → Đang xử lý, không thể hủy
/// ❌ Pay()            → Đã thanh toán
/// ❌ StartProcessing() → Đã đang xử lý rồi
/// ❌ Deliver()        → Chưa giao
/// ❌ Complete()       → Chưa giao xong

public class ProcessingState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Processing;
    public override string Description => "Đang xử lý";

    
    /// ✅ Bắt đầu giao hàng → Chuyển sang ShippingState
    
    public override IOrderState Ship()
    {
        return new ShippingState();
    }

    
    /// ✅ Hoàn tiền → Chuyển sang RefundedState
    
    public override IOrderState Refund(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hoàn tiền", nameof(reason));

        return new RefundedState();
    }
}

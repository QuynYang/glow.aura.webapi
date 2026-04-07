using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Đã thanh toán" (Paid)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ StartProcessing() → Chuyển sang ProcessingState (bắt đầu đóng gói)
/// ✅ Refund()          → Chuyển sang RefundedState (hoàn tiền nếu có vấn đề)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Confirm() → Đã qua bước xác nhận
/// ❌ Cancel()  → Đã thanh toán, phải hoàn tiền thay vì hủy
/// ❌ Pay()     → Đã thanh toán rồi
/// ❌ Ship()    → Chưa đóng gói
/// ❌ Deliver() → Chưa giao
/// ❌ Complete() → Chưa giao xong

public class PaidState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Paid;
    public override string Description => "Đã thanh toán";

    
    /// ✅ Bắt đầu xử lý/đóng gói → Chuyển sang ProcessingState
    
    public override IOrderState StartProcessing()
    {
        return new ProcessingState();
    }

    
    /// ✅ Hoàn tiền → Chuyển sang RefundedState
    /// (Đã thanh toán nên có thể yêu cầu hoàn tiền)
    
    public override IOrderState Refund(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hoàn tiền", nameof(reason));

        return new RefundedState();
    }
}

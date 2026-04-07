using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Đã xác nhận" (Confirmed)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Pay()    → Chuyển sang PaidState
/// ✅ Cancel() → Chuyển sang CancelledState (vẫn hủy được vì chưa thanh toán)
/// ✅ StartProcessing() → Cho phép nếu là đơn COD (thanh toán khi nhận hàng)
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Confirm() → Đã xác nhận rồi, không cần xác nhận lại
/// ❌ Ship()    → Chưa xử lý
/// ❌ Deliver() → Chưa giao
/// ❌ Complete() → Chưa giao xong
/// ❌ Refund()  → Chưa thanh toán

public class ConfirmedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Confirmed;
    public override string Description => "Đã xác nhận";

    
    /// ✅ Thanh toán → Chuyển sang PaidState
    
    public override IOrderState Pay(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Mã giao dịch không được để trống", nameof(transactionId));

        return new PaidState();
    }

    
    /// ✅ Hủy đơn → Chuyển sang CancelledState
    /// (Vẫn được hủy vì chưa thanh toán)
    
    public override IOrderState Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hủy đơn", nameof(reason));

        return new CancelledState();
    }

    
    /// ✅ Bắt đầu xử lý → Cho phép cho đơn COD (thanh toán khi nhận hàng)
    
    public override IOrderState StartProcessing()
    {
        // Đơn COD: Confirmed → Processing (không cần trả tiền trước)
        return new ProcessingState();
    }
}

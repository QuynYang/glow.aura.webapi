using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;


/// STATE PATTERN - Trạng thái "Thanh toán thất bại" (PaymentFailed)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// ✅ Pay()    → Thử thanh toán lại → Chuyển sang PaidState
/// ✅ Cancel() → Hủy đơn hàng → Chuyển sang CancelledState
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Confirm()         → Đã xác nhận rồi
/// ❌ StartProcessing() → Chưa thanh toán
/// ❌ Ship()            → Chưa xử lý
/// ❌ Deliver()         → Chưa giao
/// ❌ Complete()        → Chưa hoàn thành
/// ❌ Refund()          → Chưa thanh toán, không có gì để hoàn

public class PaymentFailedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.PaymentFailed;
    public override string Description => "Thanh toán thất bại";

    
    /// ✅ Thử thanh toán lại → Chuyển sang PaidState
    
    public override IOrderState Pay(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            throw new ArgumentException("Mã giao dịch không được để trống", nameof(transactionId));

        return new PaidState();
    }

    
    /// ✅ Hủy đơn hàng → Chuyển sang CancelledState
    
    public override IOrderState Cancel(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Vui lòng nhập lý do hủy đơn", nameof(reason));

        return new CancelledState();
    }
}

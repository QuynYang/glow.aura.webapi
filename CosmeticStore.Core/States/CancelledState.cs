using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;

/// STATE PATTERN - Trạng thái "Đã hủy" (Cancelled)
/// Đơn hàng đã bị hủy.

/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Tất cả → Đơn hàng đã hủy, không thể thay đổi
public class CancelledState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Cancelled;
    public override string Description => "Đã hủy";

    // Không override method nào → Tất cả đều ném lỗi từ OrderStateBase
    // Đây là trạng thái cuối cùng (Final State)
}

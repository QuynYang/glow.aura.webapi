using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.States;

/// STATE PATTERN - Trạng thái "Hoàn thành" (Completed)
/// HÀNH ĐỘNG ĐƯỢC PHÉP:
/// HÀNH ĐỘNG BỊ CẤM:
/// ❌ Tất cả → Đơn hàng đã hoàn thành, không thể thay đổi
public class CompletedState : OrderStateBase
{
    public override OrderStatus Status => OrderStatus.Completed;
    public override string Description => "Hoàn thành";

    // Không override method nào → Tất cả đều ném lỗi từ OrderStateBase
    // Đây là trạng thái cuối cùng (Final State)
}

using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;


/// STATE PATTERN - Interface cho trạng thái đơn hàng
///
/// MỤC ĐÍCH:
/// - Mỗi trạng thái (Pending, Confirmed, Paid...) là 1 class riêng
/// - Mỗi class tự biết mình được phép làm gì (confirm, cancel, pay...)
/// - Thay vì dùng if/switch kiểm tra Status, ta để State tự quyết định
/// SƠ ĐỒ CHUYỂN TRẠNG THÁI:
///
///   [Pending] ──confirm──→ [Confirmed] ──pay──→ [Paid] ──process──→ [Processing]
///      │                      │                   │                       │
///      └──cancel──→ [Cancelled] ←──cancel──┘      │                  ship │
///                                                  │                       ▼
///                                            refund│               [Shipping]
///                                                  │                       │
///                                                  ▼                deliver│
///                                             [Refunded]                   ▼
public interface IOrderState
{
    
    /// Tên trạng thái hiện tại
    
    OrderStatus Status { get; }

    
    /// Mô tả trạng thái (tiếng Việt)
    
    string Description { get; }

    
    /// Xác nhận đơn hàng
    
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Confirm();

    
    /// Hủy đơn hàng
    
    /// <param name="reason">Lý do hủy</param>
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Cancel(string reason);

    
    /// Thanh toán đơn hàng
    
    /// <param name="transactionId">Mã giao dịch</param>
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Pay(string transactionId);

    
    /// Bắt đầu xử lý/đóng gói đơn hàng
    
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState StartProcessing();

    
    /// Bắt đầu giao hàng
    
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Ship();

    
    /// Đánh dấu đã giao hàng
    
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Deliver();

    
    /// Hoàn thành đơn hàng
    
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Complete();

    
    /// Hoàn tiền đơn hàng
    
    /// <param name="reason">Lý do hoàn tiền</param>
    /// <exception cref="InvalidOperationException">Nếu không được phép ở trạng thái hiện tại</exception>
    IOrderState Refund(string reason);
}

using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;


/// Interface Repository cho Order - Kế thừa từ IGenericRepository
/// 
/// REPOSITORY PATTERN:
/// - Trừu tượng hóa truy vấn đơn hàng
/// - Thêm các method đặc thù cho nghiệp vụ Order

public interface IOrderRepository : IGenericRepository<Order>
{
    
    /// Lấy đơn hàng theo mã đơn
    
    Task<Order?> GetByOrderNumberAsync(string orderNumber);

    
    /// Lấy đơn hàng kèm chi tiết (OrderItems)
    
    Task<Order?> GetWithItemsAsync(int orderId);

    
    /// Lấy tất cả đơn hàng của một User
    
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);

    
    /// Lấy đơn hàng theo trạng thái
    
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

    
    /// Lấy đơn hàng theo khoảng thời gian
    
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

    
    /// Lấy đơn hàng chờ xử lý (Pending, Confirmed)
    
    Task<IEnumerable<Order>> GetPendingOrdersAsync();

    
    /// Đếm đơn hàng theo trạng thái
    
    Task<int> CountByStatusAsync(OrderStatus status);

    
    /// Tính tổng doanh thu theo khoảng thời gian
    
    Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);

    
    /// Lấy đơn hàng gần đây nhất của User
    
    Task<Order?> GetLatestOrderByUserAsync(int userId);
}


using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Repository cho Order - Kế thừa từ IGenericRepository
/// 
/// REPOSITORY PATTERN:
/// - Trừu tượng hóa truy vấn đơn hàng
/// - Thêm các method đặc thù cho nghiệp vụ Order
/// </summary>
public interface IOrderRepository : IGenericRepository<Order>
{
    /// <summary>
    /// Lấy đơn hàng theo mã đơn
    /// </summary>
    Task<Order?> GetByOrderNumberAsync(string orderNumber);

    /// <summary>
    /// Lấy đơn hàng kèm chi tiết (OrderItems)
    /// </summary>
    Task<Order?> GetWithItemsAsync(int orderId);

    /// <summary>
    /// Lấy tất cả đơn hàng của một User
    /// </summary>
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Lấy đơn hàng theo trạng thái
    /// </summary>
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);

    /// <summary>
    /// Lấy đơn hàng theo khoảng thời gian
    /// </summary>
    Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Lấy đơn hàng chờ xử lý (Pending, Confirmed)
    /// </summary>
    Task<IEnumerable<Order>> GetPendingOrdersAsync();

    /// <summary>
    /// Đếm đơn hàng theo trạng thái
    /// </summary>
    Task<int> CountByStatusAsync(OrderStatus status);

    /// <summary>
    /// Tính tổng doanh thu theo khoảng thời gian
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate);

    /// <summary>
    /// Lấy đơn hàng gần đây nhất của User
    /// </summary>
    Task<Order?> GetLatestOrderByUserAsync(int userId);
}


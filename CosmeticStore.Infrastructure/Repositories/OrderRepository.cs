using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;

namespace CosmeticStore.Infrastructure.Repositories;

/// <summary>
/// Repository cho Order - Kế thừa GenericRepository
/// 
/// REPOSITORY PATTERN:
/// - Che giấu phức tạp của EF Core queries
/// - Tái sử dụng code từ GenericRepository
/// - Thêm query đặc thù cho Order
/// </summary>
public class OrderRepository : GenericRepository<Order>, IOrderRepository
{
    public OrderRepository(StoreDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Lấy đơn hàng theo mã đơn
    /// </summary>
    public async Task<Order?> GetByOrderNumberAsync(string orderNumber)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);
    }

    /// <summary>
    /// Lấy đơn hàng kèm chi tiết
    /// </summary>
    public async Task<Order?> GetWithItemsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Include(o => o.User)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }

    /// <summary>
    /// Lấy tất cả đơn hàng của User
    /// </summary>
    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy đơn hàng theo trạng thái
    /// </summary>
    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.Status == status)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy đơn hàng theo khoảng thời gian
    /// </summary>
    public async Task<IEnumerable<Order>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy đơn hàng chờ xử lý
    /// </summary>
    public async Task<IEnumerable<Order>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.Status == OrderStatus.Pending || o.Status == OrderStatus.Confirmed)
            .OrderBy(o => o.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Đếm đơn hàng theo trạng thái
    /// </summary>
    public async Task<int> CountByStatusAsync(OrderStatus status)
    {
        return await _dbSet.CountAsync(o => o.Status == status);
    }

    /// <summary>
    /// Tính tổng doanh thu
    /// </summary>
    public async Task<decimal> GetTotalRevenueAsync(DateTime fromDate, DateTime toDate)
    {
        return await _dbSet
            .Where(o => o.Status == OrderStatus.Completed)
            .Where(o => o.CreatedAt >= fromDate && o.CreatedAt <= toDate)
            .SumAsync(o => o.TotalAmount);
    }

    /// <summary>
    /// Lấy đơn hàng gần đây nhất của User
    /// </summary>
    public async Task<Order?> GetLatestOrderByUserAsync(int userId)
    {
        return await _dbSet
            .Include(o => o.OrderItems)
            .Where(o => o.UserId == userId)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();
    }
}


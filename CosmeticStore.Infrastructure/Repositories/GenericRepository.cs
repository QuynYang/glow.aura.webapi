using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;

namespace CosmeticStore.Infrastructure.Repositories;

/// <summary>
/// Generic Repository - Triển khai Repository Pattern
/// 
/// Áp dụng nguyên tắc:
/// - TRỪU TƯỢNG (Abstraction): Implement IGenericRepository interface
/// - KẾ THỪA: Sử dụng Generic và làm class CHA cho các Repository cụ thể
/// 
/// Lợi ích:
/// - Một class xử lý CRUD cho tất cả Entity
/// - Tách biệt logic truy cập database khỏi business logic
/// - Dễ dàng mock trong Unit Testing
/// - Các Repository cụ thể (ProductRepository) có thể kế thừa và mở rộng
/// </summary>
/// <typeparam name="T">Kiểu Entity kế thừa từ BaseEntity</typeparam>
public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly StoreDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(StoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    #region Read Operations

    /// <summary>
    /// Lấy Entity theo Id
    /// </summary>
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Lấy tất cả Entity (Query Filter đã tự động lọc IsDeleted)
    /// </summary>
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Lấy Entity theo điều kiện lọc
    /// </summary>
    public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }

    /// <summary>
    /// Lấy Entity đầu tiên thỏa mãn điều kiện
    /// </summary>
    public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate);
    }

    /// <summary>
    /// Kiểm tra có Entity nào thỏa mãn điều kiện không
    /// </summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
    {
        return await _dbSet.AnyAsync(predicate);
    }

    /// <summary>
    /// Đếm số lượng Entity thỏa mãn điều kiện
    /// </summary>
    public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
    {
        if (predicate == null)
            return await _dbSet.CountAsync();
        
        return await _dbSet.CountAsync(predicate);
    }

    #endregion

    #region Write Operations

    /// <summary>
    /// Thêm Entity mới vào database
    /// </summary>
    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Thêm nhiều Entity cùng lúc
    /// </summary>
    public virtual async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
    }

    /// <summary>
    /// Đánh dấu Entity đã được cập nhật
    /// </summary>
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Cập nhật nhiều Entity cùng lúc
    /// </summary>
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }

    /// <summary>
    /// Xóa mềm - chỉ đánh dấu IsDeleted = true
    /// </summary>
    public virtual void SoftDelete(T entity)
    {
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Xóa cứng Entity (xóa hoàn toàn khỏi database)
    /// </summary>
    public virtual void HardDelete(T entity)
    {
        _dbSet.Remove(entity);
    }

    #endregion

    #region Unit of Work

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    public virtual async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    #endregion
}

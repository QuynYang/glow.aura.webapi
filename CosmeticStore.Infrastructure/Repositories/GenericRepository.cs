using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;

namespace CosmeticStore.Infrastructure.Repositories;

/// <summary>
/// Generic Repository - Triển khai Repository Pattern
/// 
/// Áp dụng nguyên tắc:
/// - TRỪU TƯỢNG (Abstraction): Implement IRepository interface
/// - KẾ THỪA: Sử dụng Generic <T> để tái sử dụng code cho mọi Entity
/// 
/// Lợi ích:
/// - Một class xử lý CRUD cho tất cả Entity
/// - Tách biệt logic truy cập database khỏi business logic
/// - Dễ dàng mock trong Unit Testing
/// </summary>
/// <typeparam name="T">Kiểu Entity kế thừa từ BaseEntity</typeparam>
public class GenericRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly StoreDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(StoreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    /// <summary>
    /// Lấy Entity theo Id
    /// </summary>
    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    /// <summary>
    /// Lấy tất cả Entity (Query Filter đã tự động lọc IsDeleted)
    /// </summary>
    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    /// <summary>
    /// Thêm Entity mới vào database
    /// </summary>
    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    /// <summary>
    /// Đánh dấu Entity đã được cập nhật
    /// </summary>
    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Xóa mềm - chỉ đánh dấu IsDeleted = true
    /// </summary>
    public void SoftDelete(T entity)
    {
        entity.IsDeleted = true;
        _dbSet.Update(entity);
    }

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}


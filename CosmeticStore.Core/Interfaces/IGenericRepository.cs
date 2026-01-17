using System.Linq.Expressions;
using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Generic Repository Pattern - Áp dụng tính TRỪU TƯỢNG (Abstraction)
/// 
/// Đây là interface CHA cho tất cả Repository trong hệ thống.
/// 
/// Lợi ích:
/// - Controller không phụ thuộc vào code cụ thể (EF Core, Dapper, v.v.)
/// - Dễ dàng thay đổi cách truy cập database mà không sửa Controller
/// - Hỗ trợ Unit Testing bằng cách tạo Mock Repository
/// - Giảm code lặp lại: Một interface dùng cho tất cả Entity
/// 
/// Nguyên tắc:
/// - Chứa các phương thức CRUD cơ bản dùng chung cho mọi Entity
/// - Các Repository cụ thể (ProductRepository, OrderRepository...) sẽ KẾ THỪA interface này
/// </summary>
/// <typeparam name="T">Kiểu Entity kế thừa từ BaseEntity</typeparam>
public interface IGenericRepository<T> where T : BaseEntity
{
    #region Read Operations

    /// <summary>
    /// Lấy Entity theo Id
    /// </summary>
    /// <param name="id">Id của Entity</param>
    /// <returns>Entity hoặc null nếu không tìm thấy</returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Lấy tất cả Entity (không bao gồm đã xóa mềm)
    /// </summary>
    /// <returns>Danh sách Entity</returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Lấy Entity theo điều kiện lọc
    /// </summary>
    /// <param name="predicate">Biểu thức điều kiện (ví dụ: x => x.Price > 100)</param>
    /// <returns>Danh sách Entity thỏa mãn điều kiện</returns>
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Lấy Entity đầu tiên thỏa mãn điều kiện
    /// </summary>
    /// <param name="predicate">Biểu thức điều kiện</param>
    /// <returns>Entity hoặc null</returns>
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Kiểm tra có Entity nào thỏa mãn điều kiện không
    /// </summary>
    /// <param name="predicate">Biểu thức điều kiện</param>
    /// <returns>True nếu có, False nếu không</returns>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Đếm số lượng Entity thỏa mãn điều kiện
    /// </summary>
    /// <param name="predicate">Biểu thức điều kiện (optional)</param>
    /// <returns>Số lượng Entity</returns>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null);

    #endregion

    #region Write Operations

    /// <summary>
    /// Thêm Entity mới
    /// </summary>
    /// <param name="entity">Entity cần thêm</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Thêm nhiều Entity cùng lúc
    /// </summary>
    /// <param name="entities">Danh sách Entity cần thêm</param>
    Task AddRangeAsync(IEnumerable<T> entities);

    /// <summary>
    /// Cập nhật Entity
    /// </summary>
    /// <param name="entity">Entity cần cập nhật</param>
    void Update(T entity);

    /// <summary>
    /// Cập nhật nhiều Entity cùng lúc
    /// </summary>
    /// <param name="entities">Danh sách Entity cần cập nhật</param>
    void UpdateRange(IEnumerable<T> entities);

    /// <summary>
    /// Xóa mềm Entity (đánh dấu IsDeleted = true)
    /// </summary>
    /// <param name="entity">Entity cần xóa</param>
    void SoftDelete(T entity);

    /// <summary>
    /// Xóa cứng Entity (xóa hoàn toàn khỏi database)
    /// Cảnh báo: Chỉ dùng khi thực sự cần thiết
    /// </summary>
    /// <param name="entity">Entity cần xóa</param>
    void HardDelete(T entity);

    #endregion

    #region Unit of Work

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
    Task<int> SaveChangesAsync();

    #endregion
}


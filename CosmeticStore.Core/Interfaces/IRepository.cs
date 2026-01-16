using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Repository Pattern - Áp dụng tính TRỪU TƯỢNG (Abstraction)
/// 
/// Lợi ích:
/// - Controller không phụ thuộc vào code cụ thể (EF Core, Dapper, v.v.)
/// - Dễ dàng thay đổi cách truy cập database mà không sửa Controller
/// - Hỗ trợ Unit Testing bằng cách tạo Mock Repository
/// 
/// Generic Repository: Sử dụng <T> where T : BaseEntity
/// - Một interface dùng cho tất cả Entity (Product, Order, User...)
/// - Giảm code lặp lại
/// </summary>
/// <typeparam name="T">Kiểu Entity kế thừa từ BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
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
    /// Thêm Entity mới
    /// </summary>
    /// <param name="entity">Entity cần thêm</param>
    Task AddAsync(T entity);

    /// <summary>
    /// Cập nhật Entity
    /// </summary>
    /// <param name="entity">Entity cần cập nhật</param>
    void Update(T entity);

    /// <summary>
    /// Xóa mềm Entity (đánh dấu IsDeleted = true)
    /// </summary>
    /// <param name="entity">Entity cần xóa</param>
    void SoftDelete(T entity);

    /// <summary>
    /// Lưu tất cả thay đổi vào database
    /// </summary>
    /// <returns>Số lượng bản ghi bị ảnh hưởng</returns>
    Task<int> SaveChangesAsync();
}


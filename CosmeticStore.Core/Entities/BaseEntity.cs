namespace CosmeticStore.Core.Entities;

/// <summary>
/// Lớp cơ sở cho tất cả Entity trong hệ thống.
/// Áp dụng tính KẾ THỪA (Inheritance) - Giảm code lặp lại.
/// Tất cả Entity như Product, Order, User đều kế thừa từ class này.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Khóa chính của Entity
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Thời gian tạo Entity (mặc định là thời điểm hiện tại UTC)
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Thời gian cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Đánh dấu xóa mềm (Soft Delete) - không xóa thật khỏi database
    /// </summary>
    public bool IsDeleted { get; set; } = false;
}



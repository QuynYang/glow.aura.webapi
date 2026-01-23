namespace CosmeticStore.Core.Enums;

/// <summary>
/// Enum định nghĩa vai trò người dùng trong hệ thống.
/// Sử dụng cho Authorization (phân quyền).
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Khách hàng thông thường
    /// - Xem sản phẩm, đặt hàng, xem đơn hàng của mình
    /// - Làm Skin Quiz, xem profile
    /// </summary>
    User = 0,

    /// <summary>
    /// Nhân viên bán hàng
    /// - Quyền User + Xác nhận đơn hàng, cập nhật trạng thái
    /// </summary>
    Staff = 1,

    /// <summary>
    /// Quản trị viên
    /// - Toàn quyền: CRUD sản phẩm, quản lý user, xem báo cáo
    /// </summary>
    Admin = 2
}


using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Strategy Pattern cho việc tính giá - Áp dụng tính ĐA HÌNH (Polymorphism)
/// 
/// STRATEGY PATTERN:
/// - Định nghĩa một họ các thuật toán (algorithms family)
/// - Đóng gói từng thuật toán vào class riêng
/// - Cho phép thay đổi thuật toán độc lập với client sử dụng
/// 
/// Các Strategy hiện có:
/// - StandardPricingStrategy: Giữ nguyên giá gốc (khách thường)
/// - VipPricingStrategy: Giảm giá theo VipLevel của User
/// - SkinTypePricingStrategy: Giảm giá khi loại da User phù hợp với Product
/// 
/// Lợi ích:
/// - Open/Closed Principle: Thêm chiến lược mới mà không sửa code cũ
/// - Single Responsibility: Mỗi strategy chỉ lo một cách tính giá
/// - Dễ dàng thay đổi chiến lược trong runtime
/// </summary>
public interface IPricingStrategy
{
    /// <summary>
    /// Tính giá sau khi áp dụng chiến lược
    /// </summary>
    /// <param name="product">Sản phẩm cần tính giá</param>
    /// <param name="user">Người dùng (có thể null cho khách vãng lai)</param>
    /// <returns>Giá sau khi áp dụng chiến lược</returns>
    decimal CalculatePrice(Product product, User? user);

    /// <summary>
    /// Tên chiến lược tính giá
    /// </summary>
    string StrategyName { get; }

    /// <summary>
    /// Mô tả chi tiết chiến lược
    /// </summary>
    string Description { get; }

    /// <summary>
    /// Lấy phần trăm giảm giá được áp dụng
    /// </summary>
    /// <param name="product">Sản phẩm</param>
    /// <param name="user">Người dùng</param>
    /// <returns>Phần trăm giảm (0.0 - 1.0)</returns>
    decimal GetDiscountPercent(Product product, User? user);
}

using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface Repository đặc thù cho Product - KẾ THỪA từ IGenericRepository
/// 
/// Đây là ví dụ về:
/// - Tính KẾ THỪA (Inheritance): Kế thừa tất cả method từ IGenericRepository
/// - Tính TRỪU TƯỢNG (Abstraction): Định nghĩa các method đặc thù cho Product
/// 
/// Các method đặc thù:
/// - GetBySkinType: Lọc sản phẩm theo loại da (AI Skin Quiz)
/// - GetExpiringSoon: Lọc sản phẩm cận hạn (Expiry Management)
/// - GetFlashSaleProducts: Lấy sản phẩm đang Flash Sale
/// </summary>
public interface IProductRepository : IGenericRepository<Product>
{
    #region Skin Type Filtering (AI Skin Quiz Support)

    /// <summary>
    /// Lấy danh sách sản phẩm theo loại da
    /// Dùng cho AI Skin Quiz - Gợi ý sản phẩm phù hợp
    /// </summary>
    /// <param name="skinType">Loại da cần lọc</param>
    /// <returns>Danh sách sản phẩm phù hợp với loại da</returns>
    Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType);

    /// <summary>
    /// Lấy sản phẩm phù hợp với loại da, có phân trang
    /// </summary>
    /// <param name="skinType">Loại da</param>
    /// <param name="pageNumber">Số trang (bắt đầu từ 1)</param>
    /// <param name="pageSize">Số sản phẩm mỗi trang</param>
    /// <returns>Danh sách sản phẩm đã phân trang</returns>
    Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType, int pageNumber, int pageSize);

    #endregion

    #region Expiry Management

    /// <summary>
    /// Lấy danh sách sản phẩm sắp hết hạn trong số ngày chỉ định
    /// Quan trọng: Dùng để tự động áp dụng giảm giá hoặc thông báo
    /// </summary>
    /// <param name="days">Số ngày để kiểm tra (ví dụ: 30 ngày)</param>
    /// <returns>Danh sách sản phẩm sắp hết hạn</returns>
    Task<IEnumerable<Product>> GetExpiringSoonAsync(int days);

    /// <summary>
    /// Lấy danh sách sản phẩm đã hết hạn
    /// Cảnh báo: Sản phẩm hết hạn không nên bán
    /// </summary>
    /// <returns>Danh sách sản phẩm đã hết hạn</returns>
    Task<IEnumerable<Product>> GetExpiredProductsAsync();

    /// <summary>
    /// Đếm số sản phẩm sắp hết hạn (dùng cho Dashboard Admin)
    /// </summary>
    /// <param name="days">Số ngày để kiểm tra</param>
    /// <returns>Số lượng sản phẩm sắp hết hạn</returns>
    Task<int> CountExpiringSoonAsync(int days);

    #endregion

    #region Flash Sale

    /// <summary>
    /// Lấy danh sách sản phẩm đang Flash Sale
    /// Chỉ trả về sản phẩm có IsFlashSale = true VÀ FlashSaleEndTime > Now
    /// </summary>
    /// <returns>Danh sách sản phẩm đang Flash Sale</returns>
    Task<IEnumerable<Product>> GetFlashSaleProductsAsync();

    /// <summary>
    /// Lấy sản phẩm Flash Sale có phân trang
    /// </summary>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Số sản phẩm mỗi trang</param>
    /// <returns>Danh sách sản phẩm Flash Sale đã phân trang</returns>
    Task<IEnumerable<Product>> GetFlashSaleProductsAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Đếm số sản phẩm đang Flash Sale
    /// </summary>
    /// <returns>Số lượng sản phẩm Flash Sale</returns>
    Task<int> CountFlashSaleProductsAsync();

    #endregion

    #region Brand & Category Filtering

    /// <summary>
    /// Lấy sản phẩm theo thương hiệu
    /// </summary>
    /// <param name="brand">Tên thương hiệu</param>
    /// <returns>Danh sách sản phẩm của thương hiệu</returns>
    Task<IEnumerable<Product>> GetByBrandAsync(string brand);

    /// <summary>
    /// Lấy sản phẩm theo danh mục
    /// </summary>
    /// <param name="category">Tên danh mục</param>
    /// <returns>Danh sách sản phẩm trong danh mục</returns>
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);

    /// <summary>
    /// Lấy danh sách tất cả thương hiệu có sản phẩm
    /// </summary>
    /// <returns>Danh sách tên thương hiệu</returns>
    Task<IEnumerable<string>> GetAllBrandsAsync();

    /// <summary>
    /// Lấy danh sách tất cả danh mục có sản phẩm
    /// </summary>
    /// <returns>Danh sách tên danh mục</returns>
    Task<IEnumerable<string>> GetAllCategoriesAsync();

    #endregion

    #region Price Filtering

    /// <summary>
    /// Lấy sản phẩm trong khoảng giá
    /// </summary>
    /// <param name="minPrice">Giá tối thiểu</param>
    /// <param name="maxPrice">Giá tối đa</param>
    /// <returns>Danh sách sản phẩm trong khoảng giá</returns>
    Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice);

    #endregion

    #region Stock Management

    /// <summary>
    /// Lấy sản phẩm sắp hết hàng (stock <= threshold)
    /// Dùng cho cảnh báo Admin
    /// </summary>
    /// <param name="threshold">Ngưỡng số lượng tồn kho</param>
    /// <returns>Danh sách sản phẩm sắp hết hàng</returns>
    Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold);

    /// <summary>
    /// Lấy sản phẩm đã hết hàng (stock = 0)
    /// </summary>
    /// <returns>Danh sách sản phẩm hết hàng</returns>
    Task<IEnumerable<Product>> GetOutOfStockProductsAsync();

    #endregion

    #region Search & Advanced Queries

    /// <summary>
    /// Tìm kiếm sản phẩm theo từ khóa (tên, mô tả, thương hiệu)
    /// </summary>
    /// <param name="keyword">Từ khóa tìm kiếm</param>
    /// <returns>Danh sách sản phẩm phù hợp</returns>
    Task<IEnumerable<Product>> SearchAsync(string keyword);

    /// <summary>
    /// Tìm kiếm nâng cao với nhiều điều kiện
    /// </summary>
    /// <param name="keyword">Từ khóa (optional)</param>
    /// <param name="skinType">Loại da (optional)</param>
    /// <param name="brand">Thương hiệu (optional)</param>
    /// <param name="category">Danh mục (optional)</param>
    /// <param name="minPrice">Giá tối thiểu (optional)</param>
    /// <param name="maxPrice">Giá tối đa (optional)</param>
    /// <param name="pageNumber">Số trang</param>
    /// <param name="pageSize">Số sản phẩm mỗi trang</param>
    /// <returns>Danh sách sản phẩm thỏa mãn các điều kiện</returns>
    Task<IEnumerable<Product>> AdvancedSearchAsync(
        string? keyword = null,
        SkinType? skinType = null,
        string? brand = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10);

    /// <summary>
    /// Đếm số sản phẩm thỏa mãn điều kiện tìm kiếm nâng cao
    /// Dùng cho phân trang
    /// </summary>
    Task<int> CountAdvancedSearchAsync(
        string? keyword = null,
        SkinType? skinType = null,
        string? brand = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null);

    #endregion
}


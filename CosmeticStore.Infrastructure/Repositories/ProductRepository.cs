using Microsoft.EntityFrameworkCore;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;

namespace CosmeticStore.Infrastructure.Repositories;

/// <summary>
/// Product Repository - Triển khai IProductRepository
/// 
/// Áp dụng:
/// - KẾ THỪA (Inheritance): Kế thừa GenericRepository để có sẵn CRUD cơ bản
/// - TRỪU TƯỢNG (Abstraction): Implement IProductRepository cho các query đặc thù
/// 
/// Các method đặc thù:
/// - GetBySkinTypeAsync: Lọc sản phẩm theo loại da (AI Skin Quiz)
/// - GetExpiringSoonAsync: Lọc sản phẩm cận hạn (Expiry Management)
/// - GetFlashSaleProductsAsync: Lấy sản phẩm đang Flash Sale
/// </summary>
public class ProductRepository : GenericRepository<Product>, IProductRepository
{
    public ProductRepository(StoreDbContext context) : base(context)
    {
    }

    #region Skin Type Filtering (AI Skin Quiz Support)

    /// <summary>
    /// Lấy danh sách sản phẩm theo loại da
    /// </summary>
    public async Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType)
    {
        // Nếu chọn All thì trả về tất cả sản phẩm
        if (skinType == SkinType.All)
            return await GetAllAsync();

        return await _dbSet
            .Where(p => p.SkinType == skinType || p.SkinType == SkinType.All)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy sản phẩm phù hợp với loại da, có phân trang
    /// </summary>
    public async Task<IEnumerable<Product>> GetBySkinTypeAsync(SkinType skinType, int pageNumber, int pageSize)
    {
        var query = skinType == SkinType.All 
            ? _dbSet 
            : _dbSet.Where(p => p.SkinType == skinType || p.SkinType == SkinType.All);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    #endregion

    #region Expiry Management

    /// <summary>
    /// Lấy danh sách sản phẩm sắp hết hạn trong số ngày chỉ định
    /// Quan trọng: Dùng để tự động áp dụng giảm giá hoặc thông báo
    /// </summary>
    public async Task<IEnumerable<Product>> GetExpiringSoonAsync(int days)
    {
        var warningDate = DateTime.UtcNow.AddDays(days);
        
        return await _dbSet
            .Where(p => p.ExpiryDate.HasValue 
                        && p.ExpiryDate.Value <= warningDate 
                        && p.ExpiryDate.Value > DateTime.UtcNow)
            .OrderBy(p => p.ExpiryDate)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách sản phẩm đã hết hạn
    /// </summary>
    public async Task<IEnumerable<Product>> GetExpiredProductsAsync()
    {
        return await _dbSet
            .Where(p => p.ExpiryDate.HasValue && p.ExpiryDate.Value <= DateTime.UtcNow)
            .OrderBy(p => p.ExpiryDate)
            .ToListAsync();
    }

    /// <summary>
    /// Đếm số sản phẩm sắp hết hạn (dùng cho Dashboard Admin)
    /// </summary>
    public async Task<int> CountExpiringSoonAsync(int days)
    {
        var warningDate = DateTime.UtcNow.AddDays(days);
        
        return await _dbSet
            .CountAsync(p => p.ExpiryDate.HasValue 
                            && p.ExpiryDate.Value <= warningDate 
                            && p.ExpiryDate.Value > DateTime.UtcNow);
    }

    #endregion

    #region Flash Sale

    /// <summary>
    /// Lấy danh sách sản phẩm đang Flash Sale
    /// </summary>
    public async Task<IEnumerable<Product>> GetFlashSaleProductsAsync()
    {
        return await _dbSet
            .Where(p => p.IsFlashSale 
                        && p.FlashSaleEndTime.HasValue 
                        && p.FlashSaleEndTime.Value > DateTime.UtcNow)
            .OrderBy(p => p.FlashSaleEndTime)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy sản phẩm Flash Sale có phân trang
    /// </summary>
    public async Task<IEnumerable<Product>> GetFlashSaleProductsAsync(int pageNumber, int pageSize)
    {
        return await _dbSet
            .Where(p => p.IsFlashSale 
                        && p.FlashSaleEndTime.HasValue 
                        && p.FlashSaleEndTime.Value > DateTime.UtcNow)
            .OrderBy(p => p.FlashSaleEndTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Đếm số sản phẩm đang Flash Sale
    /// </summary>
    public async Task<int> CountFlashSaleProductsAsync()
    {
        return await _dbSet
            .CountAsync(p => p.IsFlashSale 
                            && p.FlashSaleEndTime.HasValue 
                            && p.FlashSaleEndTime.Value > DateTime.UtcNow);
    }

    #endregion

    #region Brand & Category Filtering

    /// <summary>
    /// Lấy sản phẩm theo thương hiệu
    /// </summary>
    public async Task<IEnumerable<Product>> GetByBrandAsync(string brand)
    {
        return await _dbSet
            .Where(p => p.Brand != null && p.Brand.ToLower() == brand.ToLower())
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy sản phẩm theo danh mục
    /// </summary>
    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await _dbSet
            .Where(p => p.Category != null && p.Category.ToLower() == category.ToLower())
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách tất cả thương hiệu có sản phẩm
    /// </summary>
    public async Task<IEnumerable<string>> GetAllBrandsAsync()
    {
        return await _dbSet
            .Where(p => p.Brand != null)
            .Select(p => p.Brand!)
            .Distinct()
            .OrderBy(b => b)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách tất cả danh mục có sản phẩm
    /// </summary>
    public async Task<IEnumerable<string>> GetAllCategoriesAsync()
    {
        return await _dbSet
            .Where(p => p.Category != null)
            .Select(p => p.Category!)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    #endregion

    #region Price Filtering

    /// <summary>
    /// Lấy sản phẩm trong khoảng giá
    /// </summary>
    public async Task<IEnumerable<Product>> GetByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        return await _dbSet
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToListAsync();
    }

    #endregion

    #region Stock Management

    /// <summary>
    /// Lấy sản phẩm sắp hết hàng (stock <= threshold)
    /// </summary>
    public async Task<IEnumerable<Product>> GetLowStockProductsAsync(int threshold)
    {
        return await _dbSet
            .Where(p => p.Stock <= threshold && p.Stock > 0)
            .OrderBy(p => p.Stock)
            .ToListAsync();
    }

    /// <summary>
    /// Lấy sản phẩm đã hết hàng (stock = 0)
    /// </summary>
    public async Task<IEnumerable<Product>> GetOutOfStockProductsAsync()
    {
        return await _dbSet
            .Where(p => p.Stock == 0)
            .OrderByDescending(p => p.UpdatedAt)
            .ToListAsync();
    }

    #endregion

    #region Search & Advanced Queries

    /// <summary>
    /// Tìm kiếm sản phẩm theo từ khóa (tên, mô tả, thương hiệu)
    /// </summary>
    public async Task<IEnumerable<Product>> SearchAsync(string keyword)
    {
        var lowerKeyword = keyword.ToLower();
        
        return await _dbSet
            .Where(p => p.Name.ToLower().Contains(lowerKeyword)
                        || (p.Description != null && p.Description.ToLower().Contains(lowerKeyword))
                        || (p.Brand != null && p.Brand.ToLower().Contains(lowerKeyword))
                        || (p.Category != null && p.Category.ToLower().Contains(lowerKeyword)))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Tìm kiếm nâng cao với nhiều điều kiện
    /// </summary>
    public async Task<IEnumerable<Product>> AdvancedSearchAsync(
        string? keyword = null,
        SkinType? skinType = null,
        string? brand = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int pageNumber = 1,
        int pageSize = 10)
    {
        var query = BuildAdvancedSearchQuery(keyword, skinType, brand, category, minPrice, maxPrice);

        return await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Đếm số sản phẩm thỏa mãn điều kiện tìm kiếm nâng cao
    /// </summary>
    public async Task<int> CountAdvancedSearchAsync(
        string? keyword = null,
        SkinType? skinType = null,
        string? brand = null,
        string? category = null,
        decimal? minPrice = null,
        decimal? maxPrice = null)
    {
        var query = BuildAdvancedSearchQuery(keyword, skinType, brand, category, minPrice, maxPrice);
        return await query.CountAsync();
    }

    /// <summary>
    /// Helper method: Xây dựng query tìm kiếm nâng cao
    /// </summary>
    private IQueryable<Product> BuildAdvancedSearchQuery(
        string? keyword,
        SkinType? skinType,
        string? brand,
        string? category,
        decimal? minPrice,
        decimal? maxPrice)
    {
        IQueryable<Product> query = _dbSet;

        // Lọc theo từ khóa
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var lowerKeyword = keyword.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerKeyword)
                                    || (p.Description != null && p.Description.ToLower().Contains(lowerKeyword))
                                    || (p.Brand != null && p.Brand.ToLower().Contains(lowerKeyword)));
        }

        // Lọc theo loại da
        if (skinType.HasValue && skinType.Value != SkinType.All)
        {
            query = query.Where(p => p.SkinType == skinType.Value || p.SkinType == SkinType.All);
        }

        // Lọc theo thương hiệu
        if (!string.IsNullOrWhiteSpace(brand))
        {
            query = query.Where(p => p.Brand != null && p.Brand.ToLower() == brand.ToLower());
        }

        // Lọc theo danh mục
        if (!string.IsNullOrWhiteSpace(category))
        {
            query = query.Where(p => p.Category != null && p.Category.ToLower() == category.ToLower());
        }

        // Lọc theo khoảng giá
        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        return query;
    }

    #endregion
}


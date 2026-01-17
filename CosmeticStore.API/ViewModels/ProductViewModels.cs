using CosmeticStore.Core.Enums;

namespace CosmeticStore.API.ViewModels;

/// <summary>
/// ViewModel để tạo sản phẩm mới
/// Tách biệt với Entity để bảo vệ dữ liệu (không expose Entity trực tiếp ra API)
/// </summary>
public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    
    // Cosmetic-specific properties
    public SkinType SkinType { get; set; } = SkinType.All;
    public DateTime? ExpiryDate { get; set; }
    public string? Ingredients { get; set; }
    public string? UsageInstructions { get; set; }
    public string? Volume { get; set; }
}

/// <summary>
/// ViewModel để cập nhật sản phẩm
/// </summary>
public class UpdateProductRequest
{
    public string? Description { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public decimal? NewPrice { get; set; }
    
    // Cosmetic-specific properties
    public SkinType? SkinType { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Ingredients { get; set; }
    public string? UsageInstructions { get; set; }
    public string? Volume { get; set; }
}

/// <summary>
/// ViewModel để kích hoạt Flash Sale
/// </summary>
public class ActivateFlashSaleRequest
{
    public decimal DiscountPercent { get; set; }
    public DateTime EndTime { get; set; }
}

/// <summary>
/// ViewModel trả về thông tin sản phẩm
/// </summary>
public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountedPrice { get; set; }
    public int Stock { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? ImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    // Cosmetic-specific properties
    public string SkinType { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public int? DaysUntilExpiry { get; set; }
    public bool IsExpiringSoon { get; set; }
    public string? Ingredients { get; set; }
    public string? UsageInstructions { get; set; }
    public string? Volume { get; set; }
    
    // Flash Sale properties
    public bool IsFlashSale { get; set; }
    public decimal? FlashSaleDiscount { get; set; }
    public DateTime? FlashSaleEndTime { get; set; }
}

/// <summary>
/// ViewModel cho tìm kiếm nâng cao sản phẩm
/// </summary>
public class ProductSearchRequest
{
    public string? Keyword { get; set; }
    public SkinType? SkinType { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

/// <summary>
/// ViewModel trả về kết quả phân trang
/// </summary>
public class PaginatedResponse<T>
{
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

/// <summary>
/// ViewModel cho thống kê Dashboard Admin
/// </summary>
public class ProductDashboardStats
{
    public int TotalProducts { get; set; }
    public int ExpiringSoonCount { get; set; }
    public int FlashSaleCount { get; set; }
    public int LowStockCount { get; set; }
    public int OutOfStockCount { get; set; }
}

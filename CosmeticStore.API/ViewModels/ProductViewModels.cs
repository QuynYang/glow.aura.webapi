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
}


namespace CosmeticStore.Core.Entities;

/// <summary>
/// Entity đại diện cho sản phẩm mỹ phẩm.
/// Áp dụng tính ĐÓNG GÓI (Encapsulation) - Bảo vệ dữ liệu.
/// - Sử dụng private set: Dữ liệu chỉ được thay đổi thông qua các method
/// - Constructor đảm bảo object luôn hợp lệ khi khởi tạo
/// - Method UpdateStock() chứa logic nghiệp vụ thay vì để Controller xử lý
/// </summary>
public class Product : BaseEntity // Kế thừa từ BaseEntity
{
    /// <summary>
    /// Tên sản phẩm (private set - chỉ sửa được qua method)
    /// </summary>
    public string Name { get; private set; } = string.Empty;

    /// <summary>
    /// Mô tả sản phẩm
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Giá gốc sản phẩm (private set - đảm bảo không bị sửa tùy tiện)
    /// </summary>
    public decimal Price { get; private set; }

    /// <summary>
    /// Số lượng tồn kho
    /// </summary>
    public int Stock { get; private set; }

    /// <summary>
    /// Thương hiệu sản phẩm
    /// </summary>
    public string? Brand { get; private set; }

    /// <summary>
    /// Danh mục sản phẩm (ví dụ: Son môi, Kem dưỡng, Nước hoa...)
    /// </summary>
    public string? Category { get; private set; }

    /// <summary>
    /// URL hình ảnh sản phẩm
    /// </summary>
    public string? ImageUrl { get; private set; }

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected Product() { }

    /// <summary>
    /// Constructor chính - đảm bảo tính toàn vẹn dữ liệu khi tạo object
    /// </summary>
    /// <param name="name">Tên sản phẩm</param>
    /// <param name="price">Giá sản phẩm</param>
    /// <param name="stock">Số lượng tồn kho</param>
    /// <exception cref="ArgumentException">Ném ra khi dữ liệu không hợp lệ</exception>
    public Product(string name, decimal price, int stock)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Tên sản phẩm không được để trống", nameof(name));
        
        if (price < 0)
            throw new ArgumentException("Giá sản phẩm không được âm", nameof(price));
        
        if (stock < 0)
            throw new ArgumentException("Số lượng tồn kho không được âm", nameof(stock));

        Name = name;
        Price = price;
        Stock = stock;
    }

    /// <summary>
    /// Cập nhật số lượng tồn kho (ĐÓNG GÓI logic nghiệp vụ)
    /// Thay vì Controller tự ý sửa: product.Stock = product.Stock - 5
    /// Ta gọi: product.UpdateStock(-5) để đảm bảo logic được kiểm soát
    /// </summary>
    /// <param name="quantity">Số lượng thay đổi (dương: nhập kho, âm: xuất kho)</param>
    /// <exception cref="InvalidOperationException">Ném ra khi không đủ hàng tồn kho</exception>
    public void UpdateStock(int quantity)
    {
        if (Stock + quantity < 0)
            throw new InvalidOperationException($"Không đủ hàng tồn kho. Hiện có: {Stock}, yêu cầu xuất: {Math.Abs(quantity)}");
        
        Stock += quantity;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Giảm số lượng tồn kho khi bán hàng
    /// </summary>
    /// <param name="quantity">Số lượng cần giảm</param>
    public void DecreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng giảm phải lớn hơn 0", nameof(quantity));
        
        UpdateStock(-quantity);
    }

    /// <summary>
    /// Tăng số lượng tồn kho khi nhập hàng
    /// </summary>
    /// <param name="quantity">Số lượng cần tăng</param>
    public void IncreaseStock(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng tăng phải lớn hơn 0", nameof(quantity));
        
        UpdateStock(quantity);
    }

    /// <summary>
    /// Cập nhật thông tin sản phẩm
    /// </summary>
    public void UpdateInfo(string? description = null, string? brand = null, 
                          string? category = null, string? imageUrl = null)
    {
        if (description != null) Description = description;
        if (brand != null) Brand = brand;
        if (category != null) Category = category;
        if (imageUrl != null) ImageUrl = imageUrl;
        
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cập nhật giá sản phẩm
    /// </summary>
    /// <param name="newPrice">Giá mới</param>
    public void UpdatePrice(decimal newPrice)
    {
        if (newPrice < 0)
            throw new ArgumentException("Giá sản phẩm không được âm", nameof(newPrice));
        
        Price = newPrice;
        UpdatedAt = DateTime.UtcNow;
    }
}


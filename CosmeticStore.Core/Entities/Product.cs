using CosmeticStore.Core.Enums;

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
    #region Basic Properties

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

    #endregion

    #region Cosmetic-Specific Properties

    /// <summary>
    /// Loại da phù hợp - Dùng cho AI Skin Quiz và Product Filtering
    /// </summary>
    public SkinType SkinType { get; private set; } = SkinType.All;

    /// <summary>
    /// Ngày hết hạn sản phẩm - Quan trọng với mỹ phẩm
    /// Dùng để lọc sản phẩm cận hạn (GetExpiringSoon)
    /// </summary>
    public DateTime? ExpiryDate { get; private set; }

    /// <summary>
    /// Đánh dấu sản phẩm đang Flash Sale
    /// </summary>
    public bool IsFlashSale { get; private set; } = false;

    /// <summary>
    /// Phần trăm giảm giá Flash Sale (0-100)
    /// </summary>
    public decimal FlashSaleDiscount { get; private set; } = 0;

    /// <summary>
    /// Thời gian kết thúc Flash Sale
    /// </summary>
    public DateTime? FlashSaleEndTime { get; private set; }

    /// <summary>
    /// Thành phần sản phẩm (ingredients) - Quan trọng với mỹ phẩm
    /// </summary>
    public string? Ingredients { get; private set; }

    /// <summary>
    /// Hướng dẫn sử dụng
    /// </summary>
    public string? UsageInstructions { get; private set; }

    /// <summary>
    /// Dung tích/Khối lượng (ví dụ: 50ml, 100g)
    /// </summary>
    public string? Volume { get; private set; }

    #endregion

    #region Constructors

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
    /// Constructor đầy đủ với thông tin mỹ phẩm
    /// </summary>
    public Product(string name, decimal price, int stock, SkinType skinType, DateTime? expiryDate = null)
        : this(name, price, stock)
    {
        SkinType = skinType;
        ExpiryDate = expiryDate;
    }

    #endregion

    #region Stock Management Methods

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

    #endregion

    #region Update Methods

    /// <summary>
    /// Cập nhật thông tin cơ bản sản phẩm
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
    /// Cập nhật thông tin mỹ phẩm đặc thù
    /// </summary>
    public void UpdateCosmeticInfo(SkinType? skinType = null, DateTime? expiryDate = null,
                                    string? ingredients = null, string? usageInstructions = null,
                                    string? volume = null)
    {
        if (skinType.HasValue) SkinType = skinType.Value;
        if (expiryDate.HasValue) ExpiryDate = expiryDate.Value;
        if (ingredients != null) Ingredients = ingredients;
        if (usageInstructions != null) UsageInstructions = usageInstructions;
        if (volume != null) Volume = volume;
        
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

    #endregion

    #region Flash Sale Methods

    /// <summary>
    /// Kích hoạt Flash Sale cho sản phẩm
    /// </summary>
    /// <param name="discountPercent">Phần trăm giảm giá (0-100)</param>
    /// <param name="endTime">Thời gian kết thúc Flash Sale</param>
    public void ActivateFlashSale(decimal discountPercent, DateTime endTime)
    {
        if (discountPercent <= 0 || discountPercent > 100)
            throw new ArgumentException("Phần trăm giảm giá phải từ 1-100", nameof(discountPercent));
        
        if (endTime <= DateTime.UtcNow)
            throw new ArgumentException("Thời gian kết thúc phải trong tương lai", nameof(endTime));

        IsFlashSale = true;
        FlashSaleDiscount = discountPercent;
        FlashSaleEndTime = endTime;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Hủy Flash Sale
    /// </summary>
    public void DeactivateFlashSale()
    {
        IsFlashSale = false;
        FlashSaleDiscount = 0;
        FlashSaleEndTime = null;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Kiểm tra sản phẩm có đang trong Flash Sale hợp lệ không
    /// </summary>
    public bool IsInActiveFlashSale()
    {
        return IsFlashSale && FlashSaleEndTime.HasValue && FlashSaleEndTime.Value > DateTime.UtcNow;
    }

    #endregion

    #region Expiry Methods

    /// <summary>
    /// Kiểm tra sản phẩm có sắp hết hạn trong số ngày chỉ định không
    /// </summary>
    /// <param name="days">Số ngày để kiểm tra</param>
    /// <returns>True nếu sản phẩm sẽ hết hạn trong số ngày chỉ định</returns>
    public bool IsExpiringSoon(int days)
    {
        if (!ExpiryDate.HasValue) return false;
        
        var warningDate = DateTime.UtcNow.AddDays(days);
        return ExpiryDate.Value <= warningDate && ExpiryDate.Value > DateTime.UtcNow;
    }

    /// <summary>
    /// Kiểm tra sản phẩm đã hết hạn chưa
    /// </summary>
    public bool IsExpired()
    {
        return ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow;
    }

    /// <summary>
    /// Lấy số ngày còn lại trước khi hết hạn
    /// </summary>
    /// <returns>Số ngày còn lại, null nếu không có ngày hết hạn</returns>
    public int? GetDaysUntilExpiry()
    {
        if (!ExpiryDate.HasValue) return null;
        
        var days = (ExpiryDate.Value - DateTime.UtcNow).Days;
        return days > 0 ? days : 0;
    }

    #endregion
}

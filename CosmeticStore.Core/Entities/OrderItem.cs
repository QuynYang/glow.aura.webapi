namespace CosmeticStore.Core.Entities;

/// <summary>
/// Entity chi tiết đơn hàng - Mỗi item trong Order
/// 
/// OOP - ENCAPSULATION:
/// - Các property có private set
/// - Logic tính toán nằm trong method
/// </summary>
public class OrderItem : BaseEntity
{
    /// <summary>
    /// ID đơn hàng (Foreign Key)
    /// </summary>
    public int OrderId { get; private set; }

    /// <summary>
    /// ID sản phẩm (Foreign Key)
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Tên sản phẩm (snapshot tại thời điểm đặt hàng)
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Giá gốc sản phẩm (snapshot)
    /// </summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Giá sau giảm giá
    /// </summary>
    public decimal DiscountedPrice { get; private set; }

    /// <summary>
    /// Số lượng
    /// </summary>
    public int Quantity { get; private set; }

    /// <summary>
    /// Thành tiền = DiscountedPrice × Quantity
    /// </summary>
    public decimal TotalPrice => DiscountedPrice * Quantity;

    /// <summary>
    /// Số tiền được giảm = (UnitPrice - DiscountedPrice) × Quantity
    /// </summary>
    public decimal TotalDiscount => (UnitPrice - DiscountedPrice) * Quantity;

    /// <summary>
    /// Mô tả giảm giá đã áp dụng
    /// </summary>
    public string? DiscountDescription { get; private set; }

    /// <summary>
    /// Navigation property - Order
    /// </summary>
    public Order Order { get; private set; } = null!;

    /// <summary>
    /// Navigation property - Product
    /// </summary>
    public Product Product { get; private set; } = null!;

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected OrderItem() { }

    /// <summary>
    /// Constructor tạo OrderItem mới
    /// </summary>
    public OrderItem(
        int productId,
        string productName,
        decimal unitPrice,
        decimal discountedPrice,
        int quantity,
        string? discountDescription = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0", nameof(quantity));
        if (discountedPrice < 0)
            throw new ArgumentException("Giá không được âm", nameof(discountedPrice));
        if (discountedPrice > unitPrice)
            throw new ArgumentException("Giá giảm không được lớn hơn giá gốc", nameof(discountedPrice));

        ProductId = productId;
        ProductName = productName;
        UnitPrice = unitPrice;
        DiscountedPrice = discountedPrice;
        Quantity = quantity;
        DiscountDescription = discountDescription;
    }

    /// <summary>
    /// Gán OrderId (được gọi khi thêm vào Order)
    /// </summary>
    internal void SetOrderId(int orderId)
    {
        OrderId = orderId;
    }

    /// <summary>
    /// Cập nhật số lượng
    /// </summary>
    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Số lượng phải lớn hơn 0", nameof(newQuantity));
        
        Quantity = newQuantity;
        UpdatedAt = DateTime.UtcNow;
    }
}


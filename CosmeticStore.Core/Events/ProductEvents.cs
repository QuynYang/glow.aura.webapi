using CosmeticStore.Core.Enums;

namespace CosmeticStore.Core.Events;

/// <summary>
/// Event khi sản phẩm sắp hết hạn
/// 
/// OBSERVER PATTERN:
/// - Raised by: ProductExpiryCheckService (Background Service)
/// - Handled by: AdminAlertHandler, DiscountActivationHandler
/// </summary>
public class ProductExpiringSoonEvent : DomainEventBase
{
    public int ProductId { get; }
    public string ProductName { get; }
    public DateTime ExpiryDate { get; }
    public int DaysUntilExpiry { get; }
    public int CurrentStock { get; }
    public decimal CurrentPrice { get; }

    public ProductExpiringSoonEvent(
        int productId,
        string productName,
        DateTime expiryDate,
        int daysUntilExpiry,
        int currentStock,
        decimal currentPrice)
    {
        ProductId = productId;
        ProductName = productName;
        ExpiryDate = expiryDate;
        DaysUntilExpiry = daysUntilExpiry;
        CurrentStock = currentStock;
        CurrentPrice = currentPrice;
    }
}

/// <summary>
/// Event khi sản phẩm hết hạn
/// </summary>
public class ProductExpiredEvent : DomainEventBase
{
    public int ProductId { get; }
    public string ProductName { get; }
    public DateTime ExpiryDate { get; }
    public int RemainingStock { get; }

    public ProductExpiredEvent(
        int productId,
        string productName,
        DateTime expiryDate,
        int remainingStock)
    {
        ProductId = productId;
        ProductName = productName;
        ExpiryDate = expiryDate;
        RemainingStock = remainingStock;
    }
}

/// <summary>
/// Event khi sản phẩm sắp hết hàng
/// </summary>
public class ProductLowStockEvent : DomainEventBase
{
    public int ProductId { get; }
    public string ProductName { get; }
    public int CurrentStock { get; }
    public int Threshold { get; }

    public ProductLowStockEvent(
        int productId,
        string productName,
        int currentStock,
        int threshold)
    {
        ProductId = productId;
        ProductName = productName;
        CurrentStock = currentStock;
        Threshold = threshold;
    }
}

/// <summary>
/// Event khi Flash Sale được kích hoạt
/// </summary>
public class FlashSaleActivatedEvent : DomainEventBase
{
    public int ProductId { get; }
    public string ProductName { get; }
    public decimal OriginalPrice { get; }
    public decimal DiscountPercent { get; }
    public decimal SalePrice { get; }
    public DateTime StartTime { get; }
    public DateTime EndTime { get; }

    public FlashSaleActivatedEvent(
        int productId,
        string productName,
        decimal originalPrice,
        decimal discountPercent,
        decimal salePrice,
        DateTime startTime,
        DateTime endTime)
    {
        ProductId = productId;
        ProductName = productName;
        OriginalPrice = originalPrice;
        DiscountPercent = discountPercent;
        SalePrice = salePrice;
        StartTime = startTime;
        EndTime = endTime;
    }
}

/// <summary>
/// Event khi có sản phẩm mới phù hợp với loại da của user
/// </summary>
public class ProductMatchSkinTypeEvent : DomainEventBase
{
    public int ProductId { get; }
    public string ProductName { get; }
    public SkinType SkinType { get; }
    public decimal Price { get; }
    public string? ImageUrl { get; }
    public List<int> MatchingUserIds { get; }

    public ProductMatchSkinTypeEvent(
        int productId,
        string productName,
        SkinType skinType,
        decimal price,
        string? imageUrl,
        List<int> matchingUserIds)
    {
        ProductId = productId;
        ProductName = productName;
        SkinType = skinType;
        Price = price;
        ImageUrl = imageUrl;
        MatchingUserIds = matchingUserIds;
    }
}


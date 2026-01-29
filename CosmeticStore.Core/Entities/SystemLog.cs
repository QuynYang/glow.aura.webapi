using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.Entities;


/// Entity SystemLog - Lưu trữ logs trong Database
/// 
/// SINGLETON PATTERN:
/// - SystemLogger (Singleton) ghi dữ liệu vào bảng này
/// - Hỗ trợ query logs từ Admin Panel
/// 
/// Kế thừa BaseEntity:
/// - Id, CreatedAt, UpdatedAt, IsDeleted
/// </summary>
public class SystemLog : BaseEntity
{
    
    /// Thời điểm ghi log
   
    public DateTime Timestamp { get; private set; }

    
    /// Mức độ log (Debug, Info, Warning, Error, Critical)
   
    public LogLevel Level { get; private set; }

    
    /// Danh mục log (Order, Payment, Product, Review, API, System)
   
    public string Category { get; private set; } = string.Empty;

    
    /// Nội dung log
   
    public string Message { get; private set; } = string.Empty;

    
    /// Dữ liệu bổ sung (JSON)
   
    public string? Data { get; private set; }

    
    /// Thông tin Exception (nếu có)
   
    public string? ExceptionDetails { get; private set; }

    
    /// Stack trace (nếu có lỗi)
   
    public string? StackTrace { get; private set; }

    
    /// ID người dùng liên quan
   
    public int? UserId { get; private set; }

    
    /// Địa chỉ IP
   
    public string? IpAddress { get; private set; }

    
    /// Đường dẫn API request
   
    public string? RequestPath { get; private set; }

    
    /// HTTP Method (GET, POST, PUT, DELETE)
   
    public string? HttpMethod { get; private set; }

    
    /// HTTP Status Code
   
    public int? StatusCode { get; private set; }

    
    /// Thời gian xử lý (ms)
   
    public long? ResponseTimeMs { get; private set; }

    
    /// ID đối tượng liên quan (OrderId, ProductId, ReviewId...)
   
    public int? RelatedEntityId { get; private set; }

    
    /// Loại đối tượng liên quan
   
    public string? RelatedEntityType { get; private set; }

    
    /// Tên máy chủ/server
   
    public string? MachineName { get; private set; }

    
    /// Constructor mặc định cho EF Core
   
    protected SystemLog() { }

    
    /// Constructor chính - ENCAPSULATION
   
    public SystemLog(
        LogLevel level,
        string category,
        string message,
        string? data = null,
        Exception? exception = null,
        int? userId = null,
        string? ipAddress = null)
    {
        Timestamp = DateTime.UtcNow;
        Level = level;
        Category = category;
        Message = message;
        Data = data;
        UserId = userId;
        IpAddress = ipAddress;
        MachineName = Environment.MachineName;

        if (exception != null)
        {
            ExceptionDetails = exception.Message;
            StackTrace = exception.StackTrace;
        }
    }

    #region Factory Methods

    
    /// Tạo log cho hoạt động đơn hàng
   
    public static SystemLog ForOrder(
        int orderId, 
        OrderActivityType activityType, 
        string details, 
        int? userId = null)
    {
        var level = activityType switch
        {
            OrderActivityType.Cancelled => LogLevel.Warning,
            OrderActivityType.PaymentFailed => LogLevel.Error,
            _ => LogLevel.Info
        };

        return new SystemLog(level, "Order", $"{activityType}: {details}")
        {
            RelatedEntityId = orderId,
            RelatedEntityType = "Order",
            UserId = userId
        };
    }

    
    /// Tạo log cho hoạt động thanh toán
   
    public static SystemLog ForPayment(
        int orderId,
        string paymentMethod,
        PaymentActivityStatus status,
        decimal amount,
        string? transactionId = null,
        string? errorMessage = null)
    {
        var level = status switch
        {
            PaymentActivityStatus.Failed => LogLevel.Error,
            PaymentActivityStatus.Cancelled => LogLevel.Warning,
            _ => LogLevel.Info
        };

        var message = $"Payment {status}: {paymentMethod} - Amount: {amount:N0} VND";
        if (!string.IsNullOrEmpty(transactionId))
            message += $" - TxnId: {transactionId}";
        if (!string.IsNullOrEmpty(errorMessage))
            message += $" - Error: {errorMessage}";

        return new SystemLog(level, "Payment", message)
        {
            RelatedEntityId = orderId,
            RelatedEntityType = "Order",
            Data = System.Text.Json.JsonSerializer.Serialize(new
            {
                PaymentMethod = paymentMethod,
                Status = status.ToString(),
                Amount = amount,
                TransactionId = transactionId
            })
        };
    }

    
    /// Tạo log cho hoạt động sản phẩm
   
    public static SystemLog ForProduct(
        int productId,
        ProductActivityType activityType,
        string details,
        int? userId = null)
    {
        return new SystemLog(LogLevel.Info, "Product", $"{activityType}: {details}")
        {
            RelatedEntityId = productId,
            RelatedEntityType = "Product",
            UserId = userId
        };
    }

    
    /// Tạo log cho hoạt động review
   
    public static SystemLog ForReview(
        int reviewId,
        int productId,
        ReviewActivityType activityType,
        int userId,
        string? content = null)
    {
        return new SystemLog(LogLevel.Info, "Review", $"{activityType}: ReviewId={reviewId}, ProductId={productId}")
        {
            RelatedEntityId = reviewId,
            RelatedEntityType = "Review",
            UserId = userId,
            Data = content
        };
    }

    
    /// Tạo log cho API request
   
    public static SystemLog ForApiRequest(
        string endpoint,
        string method,
        int statusCode,
        long responseTimeMs,
        string? userId = null,
        string? ipAddress = null)
    {
        var level = statusCode >= 500 ? LogLevel.Error
                  : statusCode >= 400 ? LogLevel.Warning
                  : LogLevel.Info;

        return new SystemLog(level, "API", $"{method} {endpoint} - {statusCode} ({responseTimeMs}ms)")
        {
            RequestPath = endpoint,
            HttpMethod = method,
            StatusCode = statusCode,
            ResponseTimeMs = responseTimeMs,
            IpAddress = ipAddress
        };
    }

    #endregion

    #region Update Methods

    
    /// Thêm thông tin request
   
    public void SetRequestInfo(string path, string method, int statusCode, long responseTimeMs)
    {
        RequestPath = path;
        HttpMethod = method;
        StatusCode = statusCode;
        ResponseTimeMs = responseTimeMs;
    }

    #endregion
}


using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Core.Entities;

/// <summary>
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
    /// <summary>
    /// Thời điểm ghi log
    /// </summary>
    public DateTime Timestamp { get; private set; }

    /// <summary>
    /// Mức độ log (Debug, Info, Warning, Error, Critical)
    /// </summary>
    public LogLevel Level { get; private set; }

    /// <summary>
    /// Danh mục log (Order, Payment, Product, Review, API, System)
    /// </summary>
    public string Category { get; private set; } = string.Empty;

    /// <summary>
    /// Nội dung log
    /// </summary>
    public string Message { get; private set; } = string.Empty;

    /// <summary>
    /// Dữ liệu bổ sung (JSON)
    /// </summary>
    public string? Data { get; private set; }

    /// <summary>
    /// Thông tin Exception (nếu có)
    /// </summary>
    public string? ExceptionDetails { get; private set; }

    /// <summary>
    /// Stack trace (nếu có lỗi)
    /// </summary>
    public string? StackTrace { get; private set; }

    /// <summary>
    /// ID người dùng liên quan
    /// </summary>
    public int? UserId { get; private set; }

    /// <summary>
    /// Địa chỉ IP
    /// </summary>
    public string? IpAddress { get; private set; }

    /// <summary>
    /// Đường dẫn API request
    /// </summary>
    public string? RequestPath { get; private set; }

    /// <summary>
    /// HTTP Method (GET, POST, PUT, DELETE)
    /// </summary>
    public string? HttpMethod { get; private set; }

    /// <summary>
    /// HTTP Status Code
    /// </summary>
    public int? StatusCode { get; private set; }

    /// <summary>
    /// Thời gian xử lý (ms)
    /// </summary>
    public long? ResponseTimeMs { get; private set; }

    /// <summary>
    /// ID đối tượng liên quan (OrderId, ProductId, ReviewId...)
    /// </summary>
    public int? RelatedEntityId { get; private set; }

    /// <summary>
    /// Loại đối tượng liên quan
    /// </summary>
    public string? RelatedEntityType { get; private set; }

    /// <summary>
    /// Tên máy chủ/server
    /// </summary>
    public string? MachineName { get; private set; }

    /// <summary>
    /// Constructor mặc định cho EF Core
    /// </summary>
    protected SystemLog() { }

    /// <summary>
    /// Constructor chính - ENCAPSULATION
    /// </summary>
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

    /// <summary>
    /// Tạo log cho hoạt động đơn hàng
    /// </summary>
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

    /// <summary>
    /// Tạo log cho hoạt động thanh toán
    /// </summary>
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

    /// <summary>
    /// Tạo log cho hoạt động sản phẩm
    /// </summary>
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

    /// <summary>
    /// Tạo log cho hoạt động review
    /// </summary>
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

    /// <summary>
    /// Tạo log cho API request
    /// </summary>
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

    /// <summary>
    /// Thêm thông tin request
    /// </summary>
    public void SetRequestInfo(string path, string method, int statusCode, long responseTimeMs)
    {
        RequestPath = path;
        HttpMethod = method;
        StatusCode = statusCode;
        ResponseTimeMs = responseTimeMs;
    }

    #endregion
}


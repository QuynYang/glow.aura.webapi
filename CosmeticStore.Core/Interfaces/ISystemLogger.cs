namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho System Logger - SINGLETON PATTERN
/// 
/// SINGLETON PATTERN:
/// - Toàn hệ thống chỉ có 1 instance duy nhất
/// - Đảm bảo tính nhất quán khi ghi log
/// - Sử dụng AddSingleton trong DI của .NET
/// 
/// Tính năng:
/// - Ghi log vào File (logs/system-yyyy-MM-dd.log)
/// - Ghi log vào Database (bảng SystemLogs)
/// - Log levels: Debug, Info, Warning, Error, Critical
/// - Log hoạt động nghiệp vụ: Order, Payment, Product, Review
/// </summary>
public interface ISystemLogger
{
    #region Core Logging Methods

    /// <summary>
    /// Log thông tin debug (chỉ trong Development)
    /// </summary>
    void LogDebug(string message, object? data = null);

    /// <summary>
    /// Log thông tin (INFO level)
    /// </summary>
    void LogInfo(string message, object? data = null);

    /// <summary>
    /// Log cảnh báo (WARNING level)
    /// </summary>
    void LogWarning(string message, object? data = null);

    /// <summary>
    /// Log lỗi (ERROR level)
    /// </summary>
    void LogError(string message, Exception? exception = null, object? data = null);

    /// <summary>
    /// Log lỗi nghiêm trọng (CRITICAL level)
    /// </summary>
    void LogCritical(string message, Exception? exception = null, object? data = null);

    #endregion

    #region Business Activity Logging

    /// <summary>
    /// Log hoạt động đơn hàng
    /// </summary>
    void LogOrderActivity(int orderId, OrderActivityType activityType, string details, int? userId = null);

    /// <summary>
    /// Log hoạt động thanh toán
    /// </summary>
    void LogPaymentActivity(int orderId, string paymentMethod, PaymentActivityStatus status, 
        decimal amount, string? transactionId = null, string? errorMessage = null);

    /// <summary>
    /// Log hoạt động sản phẩm
    /// </summary>
    void LogProductActivity(int productId, ProductActivityType activityType, string details, int? userId = null);

    /// <summary>
    /// Log hoạt động review
    /// </summary>
    void LogReviewActivity(int reviewId, int productId, ReviewActivityType activityType, int userId, string? content = null);

    /// <summary>
    /// Log API request
    /// </summary>
    void LogApiRequest(string endpoint, string method, int statusCode, long responseTimeMs, 
        string? userId = null, string? ipAddress = null);

    #endregion

    #region Query Methods

    /// <summary>
    /// Lấy logs theo ngày
    /// </summary>
    Task<IEnumerable<SystemLogEntry>> GetLogsByDateAsync(DateTime date, LogLevel? level = null);

    /// <summary>
    /// Lấy logs theo category
    /// </summary>
    Task<IEnumerable<SystemLogEntry>> GetLogsByCategoryAsync(string category, int take = 100);

    /// <summary>
    /// Tìm kiếm logs
    /// </summary>
    Task<IEnumerable<SystemLogEntry>> SearchLogsAsync(string keyword, DateTime? fromDate = null, DateTime? toDate = null);

    #endregion
}

#region Enums

/// <summary>
/// Log Level
/// </summary>
public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3,
    Critical = 4
}

/// <summary>
/// Loại hoạt động đơn hàng
/// </summary>
public enum OrderActivityType
{
    Created,
    Updated,
    Confirmed,
    Processing,
    Shipping,
    Delivered,
    Completed,
    Cancelled,
    PaymentPending,
    PaymentSuccess,
    PaymentFailed,
    Refunded
}

/// <summary>
/// Trạng thái hoạt động thanh toán
/// </summary>
public enum PaymentActivityStatus
{
    Initiated,
    Processing,
    Success,
    Failed,
    Cancelled,
    Refunded,
    Expired
}

/// <summary>
/// Loại hoạt động sản phẩm
/// </summary>
public enum ProductActivityType
{
    Created,
    Updated,
    Deleted,
    StockUpdated,
    PriceChanged,
    FlashSaleActivated,
    FlashSaleDeactivated,
    ExpiringNotified
}

/// <summary>
/// Loại hoạt động review
/// </summary>
public enum ReviewActivityType
{
    Created,
    Updated,
    Deleted,
    Approved,
    Rejected,
    Reported
}

#endregion

#region DTOs

/// <summary>
/// Entry log trong hệ thống
/// </summary>
public class SystemLogEntry
{
    public long Id { get; set; }
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Data { get; set; }
    public string? Exception { get; set; }
    public string? UserId { get; set; }
    public string? IpAddress { get; set; }
    public string? RequestPath { get; set; }
}

#endregion


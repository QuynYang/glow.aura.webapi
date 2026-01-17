namespace CosmeticStore.Core.Interfaces;

/// <summary>
/// Interface cho Logger - Được implement bằng Singleton Pattern
/// 
/// SINGLETON PATTERN:
/// - Toàn hệ thống chỉ có 1 instance Logger
/// - Ghi log API request, lỗi thanh toán, tạo/hủy đơn, AI tư vấn
/// </summary>
public interface IAppLogger
{
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
    /// Log hoạt động đơn hàng
    /// </summary>
    void LogOrderActivity(int orderId, string action, string details, int? userId = null);

    /// <summary>
    /// Log hoạt động thanh toán
    /// </summary>
    void LogPaymentActivity(int orderId, string paymentMethod, string status, string? transactionId = null);

    /// <summary>
    /// Log Command execution
    /// </summary>
    void LogCommand(string commandName, Guid commandId, bool isSuccess, long executionTimeMs, object? data = null);
}


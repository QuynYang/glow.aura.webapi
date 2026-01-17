using CosmeticStore.Core.Interfaces;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// App Logger - Implement Singleton Pattern thông qua DI
/// 
/// SINGLETON PATTERN:
/// - Được đăng ký AddSingleton trong Program.cs
/// - Toàn hệ thống dùng chung 1 instance
/// 
/// CHỨC NĂNG:
/// - Ghi log API request
/// - Ghi log lỗi thanh toán
/// - Ghi log tạo/hủy đơn
/// - Ghi log Command execution
/// </summary>
public class AppLogger : IAppLogger
{
    private readonly ILogger<AppLogger> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AppLogger(ILogger<AppLogger> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Log thông tin
    /// </summary>
    public void LogInfo(string message, object? data = null)
    {
        if (data != null)
        {
            _logger.LogInformation("{Message} | Data: {Data}", message, SerializeData(data));
        }
        else
        {
            _logger.LogInformation("{Message}", message);
        }
    }

    /// <summary>
    /// Log cảnh báo
    /// </summary>
    public void LogWarning(string message, object? data = null)
    {
        if (data != null)
        {
            _logger.LogWarning("{Message} | Data: {Data}", message, SerializeData(data));
        }
        else
        {
            _logger.LogWarning("{Message}", message);
        }
    }

    /// <summary>
    /// Log lỗi
    /// </summary>
    public void LogError(string message, Exception? exception = null, object? data = null)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "{Message} | Data: {Data}", message, SerializeData(data));
        }
        else
        {
            _logger.LogError("{Message} | Data: {Data}", message, SerializeData(data));
        }
    }

    /// <summary>
    /// Log hoạt động đơn hàng
    /// </summary>
    public void LogOrderActivity(int orderId, string action, string details, int? userId = null)
    {
        _logger.LogInformation(
            "[ORDER] OrderId: {OrderId} | Action: {Action} | UserId: {UserId} | Details: {Details}",
            orderId, action, userId?.ToString() ?? "N/A", details);
    }

    /// <summary>
    /// Log hoạt động thanh toán
    /// </summary>
    public void LogPaymentActivity(int orderId, string paymentMethod, string status, string? transactionId = null)
    {
        _logger.LogInformation(
            "[PAYMENT] OrderId: {OrderId} | Method: {PaymentMethod} | Status: {Status} | TransactionId: {TransactionId}",
            orderId, paymentMethod, status, transactionId ?? "N/A");
    }

    /// <summary>
    /// Log Command execution
    /// </summary>
    public void LogCommand(string commandName, Guid commandId, bool isSuccess, long executionTimeMs, object? data = null)
    {
        var status = isSuccess ? "SUCCESS" : "FAILED";
        
        _logger.LogInformation(
            "[COMMAND] {CommandName} | Id: {CommandId} | Status: {Status} | Time: {ExecutionTime}ms | Data: {Data}",
            commandName, commandId, status, executionTimeMs, SerializeData(data));
    }

    /// <summary>
    /// Serialize object to JSON string
    /// </summary>
    private static string SerializeData(object? data)
    {
        if (data == null) return "null";
        
        try
        {
            return JsonSerializer.Serialize(data, _jsonOptions);
        }
        catch
        {
            return data.ToString() ?? "null";
        }
    }
}


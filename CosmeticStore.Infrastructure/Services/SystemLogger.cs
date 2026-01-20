using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.DbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CosmeticStore.Infrastructure.Services;

/// <summary>
/// System Logger - SINGLETON PATTERN
/// 
/// SINGLETON PATTERN:
/// - Được đăng ký với AddSingleton trong DI
/// - Toàn hệ thống chỉ có 1 instance duy nhất
/// - Thread-safe với ConcurrentQueue để batch writing
/// 
/// Tính năng:
/// 1. Ghi log vào File (logs/system-yyyy-MM-dd.log)
/// 2. Ghi log vào Database (bảng SystemLogs)
/// 3. Async batch writing để không block request
/// 4. Log rotation tự động theo ngày
/// 
/// OOP - ENCAPSULATION:
/// - Che giấu logic ghi file/DB phức tạp
/// - Client chỉ gọi LogInfo(), LogError()...
/// </summary>
public class SystemLogger : ISystemLogger, IDisposable
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _logDirectory;
    private readonly ConcurrentQueue<SystemLog> _logQueue;
    private readonly Timer _flushTimer;
    private readonly object _fileLock = new();
    private bool _disposed;

    // Cấu hình
    private const int BATCH_SIZE = 50;
    private const int FLUSH_INTERVAL_MS = 5000; // 5 giây

    /// <summary>
    /// Constructor - SINGLETON được tạo bởi DI Container
    /// </summary>
    public SystemLogger(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _logQueue = new ConcurrentQueue<SystemLog>();
        
        // Tạo thư mục logs
        _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }

        // Timer để flush logs định kỳ
        _flushTimer = new Timer(FlushLogsAsync, null, FLUSH_INTERVAL_MS, FLUSH_INTERVAL_MS);

        LogInfo("SystemLogger initialized", new { LogDirectory = _logDirectory });
    }

    #region Core Logging Methods

    /// <summary>
    /// Log thông tin debug (chỉ trong Development)
    /// </summary>
    public void LogDebug(string message, object? data = null)
    {
        #if DEBUG
        WriteLog(LogLevel.Debug, "System", message, data);
        #endif
    }

    /// <summary>
    /// Log thông tin (INFO level)
    /// </summary>
    public void LogInfo(string message, object? data = null)
    {
        WriteLog(LogLevel.Info, "System", message, data);
    }

    /// <summary>
    /// Log cảnh báo (WARNING level)
    /// </summary>
    public void LogWarning(string message, object? data = null)
    {
        WriteLog(LogLevel.Warning, "System", message, data);
    }

    /// <summary>
    /// Log lỗi (ERROR level)
    /// </summary>
    public void LogError(string message, Exception? exception = null, object? data = null)
    {
        WriteLog(LogLevel.Error, "System", message, data, exception);
    }

    /// <summary>
    /// Log lỗi nghiêm trọng (CRITICAL level)
    /// </summary>
    public void LogCritical(string message, Exception? exception = null, object? data = null)
    {
        WriteLog(LogLevel.Critical, "System", message, data, exception);
        
        // Critical logs được flush ngay lập tức
        FlushLogsAsync(null);
    }

    #endregion

    #region Business Activity Logging

    /// <summary>
    /// Log hoạt động đơn hàng
    /// </summary>
    public void LogOrderActivity(int orderId, OrderActivityType activityType, string details, int? userId = null)
    {
        var log = SystemLog.ForOrder(orderId, activityType, details, userId);
        EnqueueLog(log);
        WriteToFile(log);
    }

    /// <summary>
    /// Log hoạt động thanh toán
    /// </summary>
    public void LogPaymentActivity(int orderId, string paymentMethod, PaymentActivityStatus status,
        decimal amount, string? transactionId = null, string? errorMessage = null)
    {
        var log = SystemLog.ForPayment(orderId, paymentMethod, status, amount, transactionId, errorMessage);
        EnqueueLog(log);
        WriteToFile(log);

        // Payment errors cần flush ngay
        if (status == PaymentActivityStatus.Failed)
        {
            FlushLogsAsync(null);
        }
    }

    /// <summary>
    /// Log hoạt động sản phẩm
    /// </summary>
    public void LogProductActivity(int productId, ProductActivityType activityType, string details, int? userId = null)
    {
        var log = SystemLog.ForProduct(productId, activityType, details, userId);
        EnqueueLog(log);
        WriteToFile(log);
    }

    /// <summary>
    /// Log hoạt động review
    /// </summary>
    public void LogReviewActivity(int reviewId, int productId, ReviewActivityType activityType, int userId, string? content = null)
    {
        var log = SystemLog.ForReview(reviewId, productId, activityType, userId, content);
        EnqueueLog(log);
        WriteToFile(log);
    }

    /// <summary>
    /// Log API request
    /// </summary>
    public void LogApiRequest(string endpoint, string method, int statusCode, long responseTimeMs,
        string? userId = null, string? ipAddress = null)
    {
        var log = SystemLog.ForApiRequest(endpoint, method, statusCode, responseTimeMs, userId, ipAddress);
        EnqueueLog(log);
        WriteToFile(log);
    }

    #endregion

    #region Query Methods

    /// <summary>
    /// Lấy logs theo ngày
    /// </summary>
    public async Task<IEnumerable<SystemLogEntry>> GetLogsByDateAsync(DateTime date, LogLevel? level = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

        var query = dbContext.SystemLogs
            .Where(l => l.Timestamp.Date == date.Date);

        if (level.HasValue)
        {
            query = query.Where(l => l.Level == level.Value);
        }

        var logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Take(500)
            .ToListAsync();

        return logs.Select(MapToEntry);
    }

    /// <summary>
    /// Lấy logs theo category
    /// </summary>
    public async Task<IEnumerable<SystemLogEntry>> GetLogsByCategoryAsync(string category, int take = 100)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

        var logs = await dbContext.SystemLogs
            .Where(l => l.Category == category)
            .OrderByDescending(l => l.Timestamp)
            .Take(take)
            .ToListAsync();

        return logs.Select(MapToEntry);
    }

    /// <summary>
    /// Tìm kiếm logs
    /// </summary>
    public async Task<IEnumerable<SystemLogEntry>> SearchLogsAsync(string keyword, DateTime? fromDate = null, DateTime? toDate = null)
    {
        using var scope = _serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

        var query = dbContext.SystemLogs.AsQueryable();

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            query = query.Where(l => l.Message.Contains(keyword) || 
                                    (l.Data != null && l.Data.Contains(keyword)));
        }

        if (fromDate.HasValue)
        {
            query = query.Where(l => l.Timestamp >= fromDate.Value);
        }

        if (toDate.HasValue)
        {
            query = query.Where(l => l.Timestamp <= toDate.Value);
        }

        var logs = await query
            .OrderByDescending(l => l.Timestamp)
            .Take(200)
            .ToListAsync();

        return logs.Select(MapToEntry);
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Ghi log chung
    /// </summary>
    private void WriteLog(LogLevel level, string category, string message, object? data = null, Exception? exception = null)
    {
        var dataJson = data != null ? JsonSerializer.Serialize(data) : null;
        var log = new SystemLog(level, category, message, dataJson, exception);
        
        EnqueueLog(log);
        WriteToFile(log);

        // Console output cho development
        #if DEBUG
        var color = level switch
        {
            LogLevel.Debug => ConsoleColor.Gray,
            LogLevel.Info => ConsoleColor.White,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            LogLevel.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.White
        };
        
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [{level}] [{category}] {message}");
        Console.ForegroundColor = originalColor;
        #endif
    }

    /// <summary>
    /// Thêm log vào queue để batch write
    /// </summary>
    private void EnqueueLog(SystemLog log)
    {
        _logQueue.Enqueue(log);

        // Nếu queue đầy, flush ngay
        if (_logQueue.Count >= BATCH_SIZE)
        {
            Task.Run(() => FlushLogsAsync(null));
        }
    }

    /// <summary>
    /// Ghi log vào file
    /// </summary>
    private void WriteToFile(SystemLog log)
    {
        try
        {
            var fileName = $"system-{DateTime.UtcNow:yyyy-MM-dd}.log";
            var filePath = Path.Combine(_logDirectory, fileName);

            var logLine = FormatLogLine(log);

            lock (_fileLock)
            {
                File.AppendAllText(filePath, logLine + Environment.NewLine);
            }
        }
        catch (Exception ex)
        {
            // Không throw exception khi ghi log thất bại
            Console.WriteLine($"[SystemLogger] Failed to write to file: {ex.Message}");
        }
    }

    /// <summary>
    /// Format log line cho file
    /// </summary>
    private static string FormatLogLine(SystemLog log)
    {
        var sb = new StringBuilder();
        sb.Append($"[{log.Timestamp:yyyy-MM-dd HH:mm:ss.fff}]");
        sb.Append($" [{log.Level,-8}]");
        sb.Append($" [{log.Category,-10}]");
        sb.Append($" {log.Message}");

        if (!string.IsNullOrEmpty(log.Data))
        {
            sb.Append($" | Data: {log.Data}");
        }

        if (!string.IsNullOrEmpty(log.ExceptionDetails))
        {
            sb.Append($" | Exception: {log.ExceptionDetails}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Flush logs vào database (batch)
    /// </summary>
    private async void FlushLogsAsync(object? state)
    {
        if (_logQueue.IsEmpty) return;

        var logsToWrite = new List<SystemLog>();
        
        while (_logQueue.TryDequeue(out var log) && logsToWrite.Count < BATCH_SIZE * 2)
        {
            logsToWrite.Add(log);
        }

        if (logsToWrite.Count == 0) return;

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<StoreDbContext>();

            await dbContext.SystemLogs.AddRangeAsync(logsToWrite);
            await dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Re-enqueue failed logs
            foreach (var log in logsToWrite)
            {
                _logQueue.Enqueue(log);
            }
            Console.WriteLine($"[SystemLogger] Failed to flush to database: {ex.Message}");
        }
    }

    /// <summary>
    /// Map SystemLog entity sang SystemLogEntry DTO
    /// </summary>
    private static SystemLogEntry MapToEntry(SystemLog log)
    {
        return new SystemLogEntry
        {
            Id = log.Id,
            Timestamp = log.Timestamp,
            Level = log.Level,
            Category = log.Category,
            Message = log.Message,
            Data = log.Data,
            Exception = log.ExceptionDetails,
            UserId = log.UserId?.ToString(),
            IpAddress = log.IpAddress,
            RequestPath = log.RequestPath
        };
    }

    #endregion

    #region IDisposable

    /// <summary>
    /// Dispose - Flush remaining logs
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;

        _disposed = true;
        _flushTimer?.Dispose();
        
        // Flush remaining logs
        FlushLogsAsync(null);

        GC.SuppressFinalize(this);
    }

    #endregion
}


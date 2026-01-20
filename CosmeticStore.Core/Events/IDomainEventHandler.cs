namespace CosmeticStore.Core.Events;

/// <summary>
/// Interface cho Domain Event Handler - OBSERVER PATTERN
/// 
/// OBSERVER PATTERN:
/// - Mỗi Handler là một Observer
/// - Lắng nghe một loại Event cụ thể
/// - Xử lý khi Event được publish
/// 
/// Generic Type T:
/// - T là loại Event mà Handler lắng nghe
/// - Ví dụ: IDomainEventHandler<OrderCreatedEvent>
/// 
/// Dependency Injection:
/// - Tất cả handlers được đăng ký trong DI container
/// - DomainEventDispatcher sẽ resolve và gọi HandleAsync
/// </summary>
/// <typeparam name="TEvent">Loại Domain Event</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Tên handler để debug/logging
    /// </summary>
    string HandlerName { get; }

    /// <summary>
    /// Thứ tự ưu tiên (số nhỏ chạy trước)
    /// </summary>
    int Priority => 100;

    /// <summary>
    /// Xử lý event
    /// </summary>
    /// <param name="domainEvent">Event cần xử lý</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface cho Domain Event Dispatcher
/// 
/// Chức năng:
/// - Publish event đến tất cả handlers đã đăng ký
/// - Quản lý việc gọi handlers theo thứ tự priority
/// - Xử lý lỗi từ handlers
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Publish một event đến tất cả handlers
    /// </summary>
    /// <typeparam name="TEvent">Loại event</typeparam>
    /// <param name="domainEvent">Event cần publish</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent;

    /// <summary>
    /// Publish nhiều events cùng lúc
    /// </summary>
    Task PublishAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface cho Notification Service
/// Dùng chung cho các loại notification
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Gửi email
    /// </summary>
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);

    /// <summary>
    /// Gửi SMS
    /// </summary>
    Task SendSmsAsync(string phoneNumber, string message);

    /// <summary>
    /// Gửi push notification (App)
    /// </summary>
    Task SendPushNotificationAsync(int userId, string title, string message, object? data = null);

    /// <summary>
    /// Gửi thông báo cho Admin
    /// </summary>
    Task SendAdminAlertAsync(string title, string message, AlertPriority priority = AlertPriority.Normal);
}

/// <summary>
/// Mức độ ưu tiên của alert
/// </summary>
public enum AlertPriority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}


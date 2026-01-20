namespace CosmeticStore.Core.Events;

/// <summary>
/// Interface cho Domain Event - OBSERVER PATTERN
/// 
/// OBSERVER PATTERN (Domain Events):
/// - Subject (Entity) raise events khi có thay đổi
/// - Observers (Handlers) lắng nghe và phản ứng
/// - Loose coupling: Entity không biết ai đang lắng nghe
/// 
/// Workflow:
/// 1. Order được tạo → raise OrderCreatedEvent
/// 2. EmailNotificationHandler lắng nghe → Gửi email
/// 3. SmsNotificationHandler lắng nghe → Gửi SMS
/// 4. AppNotificationHandler lắng nghe → Push notification
/// 
/// Lợi ích:
/// - Single Responsibility: Handler chỉ làm 1 việc
/// - Open/Closed: Thêm handler mới không sửa code cũ
/// - Loose Coupling: Entity không phụ thuộc vào handlers
/// </summary>
public interface IDomainEvent
{
    /// <summary>
    /// ID duy nhất của event
    /// </summary>
    Guid EventId { get; }

    /// <summary>
    /// Thời điểm event xảy ra
    /// </summary>
    DateTime OccurredAt { get; }

    /// <summary>
    /// Loại event (tên class)
    /// </summary>
    string EventType { get; }
}

/// <summary>
/// Base class cho Domain Events
/// </summary>
public abstract class DomainEventBase : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredAt { get; } = DateTime.UtcNow;
    public virtual string EventType => GetType().Name;
}


using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers.Notifications;

/// <summary>
/// SMS Notification Handler - OBSERVER PATTERN
/// 
/// Lắng nghe các events và gửi SMS thông báo cho khách hàng.
/// SMS thường ngắn gọn và dùng cho thông tin quan trọng.
/// </summary>
public class OrderCreatedSmsHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "OrderCreatedSmsHandler";
    public int Priority => 20; // SMS gửi sau Email

    public OrderCreatedSmsHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Chỉ gửi SMS nếu có số điện thoại
        if (string.IsNullOrEmpty(domainEvent.UserPhone))
        {
            _logger.LogWarning($"No phone number for SMS - Order #{domainEvent.OrderNumber}");
            return;
        }

        var message = $"CosmeticStore: Đơn hàng #{domainEvent.OrderNumber} đã được tiếp nhận. " +
                     $"Tổng tiền: {domainEvent.TotalAmount:N0} VND. Cảm ơn bạn!";

        await _notificationService.SendSmsAsync(domainEvent.UserPhone, message);

        _logger.LogInfo($"Order created SMS sent to {domainEvent.UserPhone}");
    }
}

/// <summary>
/// Handler gửi SMS khi đơn hàng được xác nhận
/// </summary>
public class OrderConfirmedSmsHandler : IDomainEventHandler<OrderConfirmedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "OrderConfirmedSmsHandler";
    public int Priority => 20;

    public OrderConfirmedSmsHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderConfirmedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(domainEvent.UserPhone)) return;

        var message = $"CosmeticStore: Đơn hàng #{domainEvent.OrderNumber} đã xác nhận. " +
                     $"Dự kiến giao: {domainEvent.EstimatedDeliveryDate:dd/MM/yyyy}";

        await _notificationService.SendSmsAsync(domainEvent.UserPhone, message);

        _logger.LogInfo($"Order confirmed SMS sent to {domainEvent.UserPhone}");
    }
}

/// <summary>
/// Handler gửi SMS khi thanh toán thất bại
/// </summary>
public class PaymentFailedSmsHandler : IDomainEventHandler<PaymentFailedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "PaymentFailedSmsHandler";
    public int Priority => 5; // Urgent - gửi sớm

    public PaymentFailedSmsHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentFailedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Không có phone trong event này, cần lookup từ user
        // Tạm thời skip
        _logger.LogWarning($"Payment failed for Order #{domainEvent.OrderNumber} - SMS notification skipped (no phone)");
        await Task.CompletedTask;
    }
}

/// <summary>
/// Handler gửi SMS khi đơn hàng được giao
/// </summary>
public class OrderDeliveredSmsHandler : IDomainEventHandler<OrderDeliveredEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "OrderDeliveredSmsHandler";
    public int Priority => 20;

    public OrderDeliveredSmsHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderDeliveredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Cần lookup phone từ user
        _logger.LogInfo($"Order #{domainEvent.OrderNumber} delivered - SMS notification");
        
        // TODO: Lookup user phone and send SMS
        await Task.CompletedTask;
    }
}


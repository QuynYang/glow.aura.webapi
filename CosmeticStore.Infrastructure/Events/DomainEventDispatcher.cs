using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace CosmeticStore.Infrastructure.Events;

/// <summary>
/// Domain Event Dispatcher - OBSERVER PATTERN
/// 
/// OBSERVER PATTERN:
/// - Là trung tâm phân phối events
/// - Khi event được publish, dispatcher tìm tất cả handlers và gọi
/// - Handlers được đăng ký qua DI container
/// 
/// Workflow:
/// 1. Handler raise event: await _dispatcher.PublishAsync(new OrderCreatedEvent(...))
/// 2. Dispatcher tìm tất cả IDomainEventHandler<OrderCreatedEvent> từ DI
/// 3. Sắp xếp theo Priority
/// 4. Gọi HandleAsync cho từng handler
/// </summary>
public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ISystemLogger _logger;

    public DomainEventDispatcher(IServiceProvider serviceProvider, ISystemLogger logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Publish một event đến tất cả handlers
    /// </summary>
    public async Task PublishAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default) 
        where TEvent : IDomainEvent
    {
        var eventType = domainEvent.GetType().Name;
        _logger.LogInfo($"Publishing domain event: {eventType}", new { domainEvent.EventId });

        try
        {
            using var scope = _serviceProvider.CreateScope();
            
            // Lấy tất cả handlers cho event type này
            var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());
            var handlers = scope.ServiceProvider.GetServices(handlerType);

            if (!handlers.Any())
            {
                _logger.LogWarning($"No handlers found for event: {eventType}");
                return;
            }

            // Sắp xếp theo priority và xử lý
            var sortedHandlers = handlers
                .Cast<dynamic>()
                .OrderBy(h => (int)h.Priority)
                .ToList();

            _logger.LogInfo($"Found {sortedHandlers.Count} handlers for {eventType}");

            foreach (var handler in sortedHandlers)
            {
                try
                {
                    var handlerName = (string)handler.HandlerName;
                    _logger.LogDebug($"Executing handler: {handlerName}");

                    await handler.HandleAsync((dynamic)domainEvent, cancellationToken);

                    _logger.LogInfo($"Handler {handlerName} completed for {eventType}");
                }
                catch (Exception ex)
                {
                    var handlerName = (string)handler.HandlerName;
                    _logger.LogError($"Handler {handlerName} failed for {eventType}", ex);
                    
                    // Tiếp tục với handler tiếp theo (không throw)
                    // Có thể thay đổi behavior tùy yêu cầu
                }
            }

            _logger.LogInfo($"Domain event {eventType} published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to publish domain event: {eventType}", ex);
            throw;
        }
    }

    /// <summary>
    /// Publish nhiều events cùng lúc
    /// </summary>
    public async Task PublishAllAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken = default)
    {
        foreach (var domainEvent in domainEvents)
        {
            // Sử dụng reflection để gọi đúng generic method
            var method = typeof(DomainEventDispatcher)
                .GetMethod(nameof(PublishAsync))!
                .MakeGenericMethod(domainEvent.GetType());

            await (Task)method.Invoke(this, new object[] { domainEvent, cancellationToken })!;
        }
    }
}


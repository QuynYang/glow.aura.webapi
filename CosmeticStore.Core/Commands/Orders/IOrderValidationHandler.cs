namespace CosmeticStore.Core.Commands.Orders;

public interface IOrderValidationHandler
{
    IOrderValidationHandler SetNext(IOrderValidationHandler next);
    Task<OrderValidationResult> ValidateAsync(OrderValidationContext context, CancellationToken cancellationToken = default);
}

public abstract class OrderValidationHandler : IOrderValidationHandler
{
    private IOrderValidationHandler? _next;

    public IOrderValidationHandler SetNext(IOrderValidationHandler next)
    {
        _next = next;
        return next;
    }

    public async Task<OrderValidationResult> ValidateAsync(OrderValidationContext context, CancellationToken cancellationToken = default)
    {
        var currentResult = await ValidateCurrentAsync(context, cancellationToken);
        if (!currentResult.IsValid)
        {
            return currentResult;
        }

        if (_next == null)
        {
            return OrderValidationResult.Success();
        }

        return await _next.ValidateAsync(context, cancellationToken);
    }

    protected abstract Task<OrderValidationResult> ValidateCurrentAsync(OrderValidationContext context, CancellationToken cancellationToken);
}

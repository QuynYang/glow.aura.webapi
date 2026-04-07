using CosmeticStore.Core.Commands.Orders;

namespace CosmeticStore.Infrastructure.Handlers.Validations.Orders;

public class CartValidator : OrderValidationHandler
{
    protected override Task<OrderValidationResult> ValidateCurrentAsync(OrderValidationContext context, CancellationToken cancellationToken)
    {
        if (!context.Command.Items.Any())
        {
            return Task.FromResult(OrderValidationResult.Failure("Đơn hàng phải có ít nhất 1 sản phẩm", "EMPTY_ORDER"));
        }

        var validationErrors = new Dictionary<string, string[]>();
        foreach (var item in context.Command.Items)
        {
            if (item.Quantity <= 0)
            {
                validationErrors[$"item_{item.ProductId}"] = ["Số lượng sản phẩm phải lớn hơn 0"];
            }
        }

        if (validationErrors.Any())
        {
            return Task.FromResult(OrderValidationResult.ValidationFailure(validationErrors));
        }

        return Task.FromResult(OrderValidationResult.Success());
    }
}

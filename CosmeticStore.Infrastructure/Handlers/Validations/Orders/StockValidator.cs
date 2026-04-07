using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers.Validations.Orders;

public class StockValidator : OrderValidationHandler
{
    private readonly IProductRepository _productRepository;

    public StockValidator(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    protected override async Task<OrderValidationResult> ValidateCurrentAsync(OrderValidationContext context, CancellationToken cancellationToken)
    {
        var validationErrors = new Dictionary<string, string[]>();

        foreach (var item in context.Command.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);

            if (product == null)
            {
                validationErrors[$"product_{item.ProductId}"] = [$"Sản phẩm ID {item.ProductId} không tồn tại"];
                continue;
            }

            if (product.Stock < item.Quantity)
            {
                validationErrors[$"product_{item.ProductId}"] =
                    [$"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm, không đủ {item.Quantity}"];
                continue;
            }

            if (product.IsExpired())
            {
                validationErrors[$"product_{item.ProductId}"] = [$"Sản phẩm '{product.Name}' đã hết hạn sử dụng"];
                continue;
            }

            context.Products[item.ProductId] = product;
        }

        if (validationErrors.Any())
        {
            return OrderValidationResult.ValidationFailure(validationErrors);
        }

        return OrderValidationResult.Success();
    }
}

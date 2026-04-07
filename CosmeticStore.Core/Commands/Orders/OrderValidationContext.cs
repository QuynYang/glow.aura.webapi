using CosmeticStore.Core.Entities;

namespace CosmeticStore.Core.Commands.Orders;

public class OrderValidationContext
{
    public required CreateOrderCommand Command { get; init; }
    public User? User { get; set; }
    public Dictionary<int, Product> Products { get; } = new();
}

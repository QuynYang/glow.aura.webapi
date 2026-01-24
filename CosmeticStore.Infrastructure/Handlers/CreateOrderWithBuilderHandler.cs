using System.Diagnostics;
using CosmeticStore.Core.Builders;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers;

/// <summary>
/// Handler xử lý CreateOrderCommand - Sử dụng BUILDER PATTERN
/// 
/// BUILDER PATTERN:
/// - Thay vì code dài dòng để tạo Order
/// - Sử dụng OrderBuilder với Fluent Interface
/// 
/// So sánh với CreateOrderCommandHandler (cũ):
/// - Cũ: 80+ dòng code xử lý trong handler
/// - Mới: ~30 dòng code, logic xây dựng trong Builder
/// 
/// Kết hợp:
/// - BUILDER: Xây dựng Order từng bước
/// - STRATEGY: Tính giá theo VIP (trong Builder)
/// - DECORATOR: Cộng dồn giảm giá (trong Builder)
/// - COMMAND: Encapsulate request
/// </summary>
public class CreateOrderWithBuilderHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IOrderBuilder _orderBuilder;
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IAppLogger _logger;

    public CreateOrderWithBuilderHandler(
        IOrderBuilder orderBuilder,
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IGenericRepository<User> userRepository,
        IAppLogger logger)
    {
        _orderBuilder = orderBuilder;
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý tạo đơn hàng sử dụng Builder Pattern
    /// </summary>
    public async Task<CommandResult<CreateOrderResult>> HandleAsync(
        CreateOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Step 1: Validate User
            var user = await _userRepository.GetByIdAsync(command.UserId);
            if (user == null)
            {
                return CommandResult<CreateOrderResult>.Failure(
                    "Không tìm thấy thông tin khách hàng",
                    "USER_NOT_FOUND");
            }

            // Step 2: Validate Items
            if (!command.Items.Any())
            {
                return CommandResult<CreateOrderResult>.Failure(
                    "Đơn hàng phải có ít nhất 1 sản phẩm",
                    "EMPTY_ORDER");
            }

            // Step 3: Convert to CartItems và load Products
            var cartItems = new List<CartItem>();
            foreach (var item in command.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    return CommandResult<CreateOrderResult>.Failure(
                        $"Sản phẩm ID {item.ProductId} không tồn tại",
                        "PRODUCT_NOT_FOUND");
                }

                cartItems.Add(new CartItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Product = product
                });
            }

            // ═══════════════════════════════════════════════════════════════════
            // BUILDER PATTERN: Xây dựng Order từng bước với Fluent Interface
            // ═══════════════════════════════════════════════════════════════════
            Order order;
            try
            {
                order = _orderBuilder
                    .Reset()                                                    // Reset builder state
                    .WithUser(user)                                             // Set User (VIP level, SkinType)
                    .WithItems(cartItems)                                       // Thêm items (tính giá Strategy+Decorator)
                    .WithShippingAddress(                                       // Set địa chỉ giao hàng
                        command.ShippingAddress,
                        command.ShippingPhone,
                        command.ReceiverName)
                    .WithPaymentMethod(command.PaymentMethod)                   // Set phương thức thanh toán
                    .WithVoucher(command.CouponCode)                            // Áp dụng voucher (optional)
                    .WithNotes(command.Notes)                                   // Thêm ghi chú (optional)
                    .Build();                                                   // Validate & Build Order
            }
            catch (InvalidOperationException ex)
            {
                // Builder validation failed
                return CommandResult<CreateOrderResult>.Failure(
                    ex.Message,
                    "VALIDATION_FAILED");
            }

            // Step 4: Trừ tồn kho
            foreach (var item in command.Items)
            {
                var product = cartItems.First(c => c.ProductId == item.ProductId).Product!;
                product.UpdateStock(-item.Quantity);
                _productRepository.Update(product);
            }

            // Step 5: Lưu vào Database
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Step 6: Ghi Log
            _logger.LogOrderActivity(
                orderId: order.Id,
                action: "CREATE_WITH_BUILDER",
                details: $"Đơn hàng {order.OrderNumber} được tạo bằng Builder Pattern, " +
                         $"{order.OrderItems.Count} sản phẩm, " +
                         $"tổng tiền: {order.TotalAmount:N0} VND",
                userId: command.UserId
            );

            stopwatch.Stop();
            _logger.LogCommand(
                commandName: $"{nameof(CreateOrderCommand)}_Builder",
                commandId: command.CommandId,
                isSuccess: true,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { OrderId = order.Id, OrderNumber = order.OrderNumber }
            );

            // Step 7: Trả về kết quả
            return CommandResult<CreateOrderResult>.Success(new CreateOrderResult
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentUrl = command.PaymentMethod.RequiresOnlinePayment()
                    ? $"/api/orders/{order.Id}/pay"
                    : null,
                Message = $"Đơn hàng {order.OrderNumber} đã được tạo thành công! (Builder Pattern)"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Lỗi tạo đơn hàng (Builder) cho UserId: {command.UserId}", ex);

            return CommandResult<CreateOrderResult>.Failure(
                "Có lỗi xảy ra khi tạo đơn hàng. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }
}

/// <summary>
/// Extension methods cho Builder Pattern demo
/// </summary>
public static class OrderBuilderExtensions
{
    /// <summary>
    /// Preview order trước khi tạo (không lưu DB)
    /// </summary>
    public static async Task<OrderBuildResult> PreviewOrderAsync(
        this IOrderBuilder builder,
        IGenericRepository<User> userRepository,
        IProductRepository productRepository,
        int userId,
        IEnumerable<OrderItemInput> items,
        string? couponCode = null)
    {
        var user = await userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new InvalidOperationException("User không tồn tại");

        var cartItems = new List<CartItem>();
        foreach (var item in items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product != null)
            {
                cartItems.Add(new CartItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Product = product
                });
            }
        }

        // Cast để gọi BuildWithDetails
        if (builder is Builders.OrderBuilder orderBuilder)
        {
            orderBuilder
                .Reset()
                .WithUser(user)
                .WithItems(cartItems)
                .WithVoucher(couponCode);

            return orderBuilder.Preview();
        }

        throw new InvalidOperationException("Builder không hỗ trợ Preview");
    }
}


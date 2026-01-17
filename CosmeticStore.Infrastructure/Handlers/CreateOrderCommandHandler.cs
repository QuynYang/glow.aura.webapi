using System.Diagnostics;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Entities;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers;

/// <summary>
/// Handler xử lý CreateOrderCommand
/// 
/// COMMAND PATTERN - SINGLE RESPONSIBILITY:
/// - Chỉ làm 1 việc: Tạo đơn hàng
/// - Workflow: Validate → Tính giá → Lưu DB → Log
/// 
/// Sử dụng:
/// - IProductRepository: Lấy thông tin sản phẩm, validate tồn kho
/// - IOrderRepository: Lưu đơn hàng
/// - IPricingService: Tính giá (Strategy + Decorator Pattern)
/// - IAppLogger: Ghi log (Singleton Pattern)
/// </summary>
public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IGenericRepository<User> _userRepository;
    private readonly IPricingService _pricingService;
    private readonly IAppLogger _logger;

    public CreateOrderCommandHandler(
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IGenericRepository<User> userRepository,
        IPricingService pricingService,
        IAppLogger logger)
    {
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _userRepository = userRepository;
        _pricingService = pricingService;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý tạo đơn hàng
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

            // Step 3: Validate Products & Stock
            var orderItems = new List<OrderItem>();
            var validationErrors = new Dictionary<string, string[]>();

            foreach (var item in command.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                
                if (product == null)
                {
                    validationErrors[$"product_{item.ProductId}"] = 
                        new[] { $"Sản phẩm ID {item.ProductId} không tồn tại" };
                    continue;
                }

                // Validate stock
                if (product.Stock < item.Quantity)
                {
                    validationErrors[$"product_{item.ProductId}"] = 
                        new[] { $"Sản phẩm '{product.Name}' chỉ còn {product.Stock} sản phẩm, không đủ {item.Quantity}" };
                    continue;
                }

                // Validate expiry
                if (product.IsExpired())
                {
                    validationErrors[$"product_{item.ProductId}"] = 
                        new[] { $"Sản phẩm '{product.Name}' đã hết hạn sử dụng" };
                    continue;
                }

                // Step 4: Tính giá (dùng PricingService từ Phase 2)
                var pricingResult = _pricingService.CalculateFinalPrice(
                    product, 
                    user, 
                    command.CouponCode);

                // Tạo OrderItem với giá đã tính
                var orderItem = new OrderItem(
                    productId: product.Id,
                    productName: product.Name,
                    unitPrice: product.Price,
                    discountedPrice: pricingResult.FinalPrice,
                    quantity: item.Quantity,
                    discountDescription: pricingResult.AppliedDiscounts.Any() 
                        ? string.Join(", ", pricingResult.AppliedDiscounts.Select(d => d.DiscountType))
                        : null
                );

                orderItems.Add(orderItem);

                // Trừ tồn kho (Encapsulation - dùng method trong Product)
                product.UpdateStock(-item.Quantity);
                _productRepository.Update(product);
            }

            // Kiểm tra validation errors
            if (validationErrors.Any())
            {
                return CommandResult<CreateOrderResult>.ValidationFailure(validationErrors);
            }

            // Step 5: Tạo Order Entity
            var order = new Order(
                userId: command.UserId,
                shippingAddress: command.ShippingAddress,
                shippingPhone: command.ShippingPhone,
                receiverName: command.ReceiverName,
                paymentMethod: command.PaymentMethod,
                notes: command.Notes,
                couponCode: command.CouponCode
            );

            // Thêm items vào order
            foreach (var item in orderItems)
            {
                order.AddItem(item);
            }

            // Tính phí ship (có thể thêm logic phức tạp sau)
            var shippingFee = CalculateShippingFee(order.SubTotal);
            order.SetShippingFee(shippingFee);

            // Step 6: Lưu vào Database
            await _orderRepository.AddAsync(order);
            await _orderRepository.SaveChangesAsync();

            // Step 7: Ghi Log (Singleton Pattern)
            _logger.LogOrderActivity(
                orderId: order.Id,
                action: "CREATE",
                details: $"Đơn hàng {order.OrderNumber} được tạo với {orderItems.Count} sản phẩm, " +
                         $"tổng tiền: {order.TotalAmount:N0} VND",
                userId: command.UserId
            );

            stopwatch.Stop();
            _logger.LogCommand(
                commandName: nameof(CreateOrderCommand),
                commandId: command.CommandId,
                isSuccess: true,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { OrderId = order.Id, OrderNumber = order.OrderNumber }
            );

            // Step 8: Trả về kết quả
            return CommandResult<CreateOrderResult>.Success(new CreateOrderResult
            {
                OrderId = order.Id,
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                Status = order.Status,
                PaymentUrl = command.PaymentMethod.RequiresOnlinePayment() 
                    ? $"/api/orders/{order.Id}/pay" 
                    : null,
                Message = $"Đơn hàng {order.OrderNumber} đã được tạo thành công!"
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Lỗi tạo đơn hàng cho UserId: {command.UserId}", ex);
            _logger.LogCommand(
                commandName: nameof(CreateOrderCommand),
                commandId: command.CommandId,
                isSuccess: false,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { Error = ex.Message }
            );

            return CommandResult<CreateOrderResult>.Failure(
                "Có lỗi xảy ra khi tạo đơn hàng. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }

    /// <summary>
    /// Tính phí vận chuyển (logic đơn giản)
    /// </summary>
    private decimal CalculateShippingFee(decimal subtotal)
    {
        // Free ship cho đơn >= 500k
        if (subtotal >= 500000)
            return 0;
        
        // Phí ship cố định
        return 30000;
    }
}


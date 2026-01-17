using System.Diagnostics;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers;

/// <summary>
/// Handler xử lý CancelOrderCommand
/// 
/// COMMAND PATTERN - SINGLE RESPONSIBILITY:
/// - Chỉ làm 1 việc: Hủy đơn hàng
/// - Workflow: Validate → Update status → Hoàn tồn kho → Log
/// 
/// Business Rules:
/// - Chỉ hủy được đơn Pending, Confirmed, PaymentFailed
/// - Hoàn lại stock cho các sản phẩm
/// - Nếu đã thanh toán -> flag RequiresRefund
/// </summary>
public class CancelOrderCommandHandler : ICommandHandler<CancelOrderCommand, CancelOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IAppLogger _logger;

    public CancelOrderCommandHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IAppLogger logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý hủy đơn hàng
    /// </summary>
    public async Task<CommandResult<CancelOrderResult>> HandleAsync(
        CancelOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Step 1: Lấy đơn hàng kèm chi tiết
            var order = await _orderRepository.GetWithItemsAsync(command.OrderId);
            
            if (order == null)
            {
                return CommandResult<CancelOrderResult>.Failure(
                    "Không tìm thấy đơn hàng",
                    "ORDER_NOT_FOUND");
            }

            // Step 2: Kiểm tra quyền hủy (User chỉ hủy được đơn của mình)
            if (!command.IsCancelledByAdmin && order.UserId != command.CancelledByUserId)
            {
                return CommandResult<CancelOrderResult>.Failure(
                    "Bạn không có quyền hủy đơn hàng này",
                    "UNAUTHORIZED");
            }

            // Step 3: Kiểm tra trạng thái có thể hủy không
            if (!order.Status.CanCancel())
            {
                return CommandResult<CancelOrderResult>.Failure(
                    $"Không thể hủy đơn hàng ở trạng thái '{order.Status.GetDescription()}'",
                    "INVALID_STATUS");
            }

            // Step 4: Kiểm tra xem có cần hoàn tiền không
            bool requiresRefund = order.Status == OrderStatus.Paid;
            decimal refundAmount = requiresRefund ? order.TotalAmount : 0;

            // Step 5: Hoàn lại tồn kho (Encapsulation - dùng method trong Product)
            foreach (var item in order.OrderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    // Cộng lại stock
                    product.UpdateStock(item.Quantity);
                    _productRepository.Update(product);

                    _logger.LogInfo(
                        $"Hoàn tồn kho sản phẩm {product.Name}: +{item.Quantity}",
                        new { ProductId = product.Id, OrderId = order.Id });
                }
            }

            // Step 6: Hủy đơn hàng (Domain method trong Order entity)
            order.Cancel(command.Reason);
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            // Step 7: Ghi Log
            _logger.LogOrderActivity(
                orderId: order.Id,
                action: "CANCEL",
                details: $"Đơn hàng {order.OrderNumber} bị hủy. Lý do: {command.Reason}. " +
                         $"Hoàn tiền: {(requiresRefund ? $"{refundAmount:N0} VND" : "Không")}",
                userId: command.CancelledByUserId
            );

            stopwatch.Stop();
            _logger.LogCommand(
                commandName: nameof(CancelOrderCommand),
                commandId: command.CommandId,
                isSuccess: true,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { OrderId = order.Id, RequiresRefund = requiresRefund }
            );

            // Step 8: Trả về kết quả
            return CommandResult<CancelOrderResult>.Success(new CancelOrderResult
            {
                OrderNumber = order.OrderNumber,
                RequiresRefund = requiresRefund,
                RefundAmount = refundAmount,
                Message = requiresRefund 
                    ? $"Đơn hàng {order.OrderNumber} đã hủy. Số tiền {refundAmount:N0} VND sẽ được hoàn trong 3-5 ngày làm việc."
                    : $"Đơn hàng {order.OrderNumber} đã hủy thành công.",
                CancelledAt = order.CancelledAt ?? DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Lỗi hủy đơn hàng ID: {command.OrderId}", ex);
            _logger.LogCommand(
                commandName: nameof(CancelOrderCommand),
                commandId: command.CommandId,
                isSuccess: false,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { Error = ex.Message }
            );

            return CommandResult<CancelOrderResult>.Failure(
                "Có lỗi xảy ra khi hủy đơn hàng. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }
}


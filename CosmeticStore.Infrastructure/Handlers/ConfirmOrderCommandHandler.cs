using System.Diagnostics;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers;

/// <summary>
/// Handler xử lý ConfirmOrderCommand
/// 
/// COMMAND PATTERN - SINGLE RESPONSIBILITY:
/// - Chỉ làm 1 việc: Xác nhận đơn hàng
/// - Workflow: Validate → Update status → Update shipping → Log
/// 
/// Business Rules:
/// - Chỉ Admin mới có quyền xác nhận
/// - Chỉ xác nhận được đơn Pending
/// - Có thể cập nhật phí ship và ngày giao dự kiến
/// </summary>
public class ConfirmOrderCommandHandler : ICommandHandler<ConfirmOrderCommand, ConfirmOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IAppLogger _logger;

    public ConfirmOrderCommandHandler(
        IOrderRepository orderRepository,
        IAppLogger logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý xác nhận đơn hàng
    /// </summary>
    public async Task<CommandResult<ConfirmOrderResult>> HandleAsync(
        ConfirmOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Step 1: Lấy đơn hàng kèm chi tiết
            var order = await _orderRepository.GetWithItemsAsync(command.OrderId);
            
            if (order == null)
            {
                return CommandResult<ConfirmOrderResult>.Failure(
                    "Không tìm thấy đơn hàng",
                    "ORDER_NOT_FOUND");
            }

            // Step 2: Kiểm tra trạng thái
            if (!order.Status.CanConfirm())
            {
                return CommandResult<ConfirmOrderResult>.Failure(
                    $"Không thể xác nhận đơn hàng ở trạng thái '{order.Status.GetDescription()}'",
                    "INVALID_STATUS");
            }

            // Step 3: Cập nhật phí ship nếu có
            if (command.ShippingFee.HasValue)
            {
                order.SetShippingFee(command.ShippingFee.Value);
            }

            // Step 4: Xác nhận đơn hàng (Domain method)
            order.Confirm();
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            // Step 5: Ghi Log
            _logger.LogOrderActivity(
                orderId: order.Id,
                action: "CONFIRM",
                details: $"Đơn hàng {order.OrderNumber} được xác nhận bởi Admin ID: {command.ConfirmedByAdminId}. " +
                         $"Tổng tiền: {order.TotalAmount:N0} VND. " +
                         $"Giao dự kiến: {order.EstimatedDeliveryDate:dd/MM/yyyy}",
                userId: command.ConfirmedByAdminId
            );

            stopwatch.Stop();
            _logger.LogCommand(
                commandName: nameof(ConfirmOrderCommand),
                commandId: command.CommandId,
                isSuccess: true,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { OrderId = order.Id, TotalAmount = order.TotalAmount }
            );

            // Step 6: Trả về kết quả
            bool requiresOnlinePayment = order.PaymentMethod.RequiresOnlinePayment();
            
            return CommandResult<ConfirmOrderResult>.Success(new ConfirmOrderResult
            {
                OrderNumber = order.OrderNumber,
                TotalAmount = order.TotalAmount,
                ShippingFee = order.ShippingFee,
                EstimatedDeliveryDate = order.EstimatedDeliveryDate,
                RequiresOnlinePayment = requiresOnlinePayment,
                PaymentUrl = requiresOnlinePayment ? $"/api/orders/{order.Id}/pay" : null,
                Message = requiresOnlinePayment
                    ? $"Đơn hàng {order.OrderNumber} đã xác nhận. Vui lòng thanh toán {order.TotalAmount:N0} VND."
                    : $"Đơn hàng {order.OrderNumber} đã xác nhận. Thanh toán khi nhận hàng."
            });
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Lỗi xác nhận đơn hàng ID: {command.OrderId}", ex);
            _logger.LogCommand(
                commandName: nameof(ConfirmOrderCommand),
                commandId: command.CommandId,
                isSuccess: false,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { Error = ex.Message }
            );

            return CommandResult<ConfirmOrderResult>.Failure(
                "Có lỗi xảy ra khi xác nhận đơn hàng. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }
}


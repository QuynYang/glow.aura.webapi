using System.Diagnostics;
using CosmeticStore.Core.Commands;
using CosmeticStore.Core.Commands.Orders;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.Services;

namespace CosmeticStore.Infrastructure.Handlers;

/// <summary>
/// Handler xử lý PayOrderCommand
/// 
/// COMMAND PATTERN + FACTORY PATTERN:
/// - Sử dụng PaymentFactory để tạo đúng Payment Service
/// - Workflow: Validate → Get Payment Service → Process → Update Order → Log
/// 
/// Business Rules:
/// - Chỉ thanh toán được đơn Confirmed hoặc PaymentFailed
/// - Gọi cổng thanh toán tương ứng (Momo, VNPay, ZaloPay)
/// - COD không cần thanh toán online
/// </summary>
public class PayOrderCommandHandler : ICommandHandler<PayOrderCommand, PayOrderResult>
{
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentFactory _paymentFactory;
    private readonly IAppLogger _logger;

    public PayOrderCommandHandler(
        IOrderRepository orderRepository,
        PaymentFactory paymentFactory,
        IAppLogger logger)
    {
        _orderRepository = orderRepository;
        _paymentFactory = paymentFactory;
        _logger = logger;
    }

    /// <summary>
    /// Xử lý thanh toán đơn hàng
    /// </summary>
    public async Task<CommandResult<PayOrderResult>> HandleAsync(
        PayOrderCommand command,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            // Step 1: Lấy đơn hàng
            var order = await _orderRepository.GetWithItemsAsync(command.OrderId);
            
            if (order == null)
            {
                return CommandResult<PayOrderResult>.Failure(
                    "Không tìm thấy đơn hàng",
                    "ORDER_NOT_FOUND");
            }

            // Step 2: Kiểm tra trạng thái
            if (!order.Status.CanPay())
            {
                return CommandResult<PayOrderResult>.Failure(
                    $"Không thể thanh toán đơn hàng ở trạng thái '{order.Status.GetDescription()}'",
                    "INVALID_STATUS");
            }

            // Step 3: Xử lý COD (không cần thanh toán online)
            if (command.PaymentMethod == PaymentMethod.COD)
            {
                // COD - đánh dấu chờ giao hàng
                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: "COD",
                    status: "PENDING_DELIVERY");

                return CommandResult<PayOrderResult>.Success(new PayOrderResult
                {
                    IsSuccess = true,
                    Amount = order.TotalAmount,
                    PaymentMethod = PaymentMethod.COD,
                    Message = "Đơn hàng sẽ được thanh toán khi nhận hàng."
                });
            }

            // Step 4: Lấy Payment Service từ Factory (FACTORY PATTERN)
            var paymentMethodString = command.PaymentMethod switch
            {
                PaymentMethod.Momo => "MOMO",
                PaymentMethod.VNPay => "VNPAY",
                PaymentMethod.ZaloPay => "ZALOPAY",
                PaymentMethod.BankTransfer => "BANK",
                _ => throw new ArgumentException($"Phương thức thanh toán không được hỗ trợ: {command.PaymentMethod}")
            };

            var paymentService = _paymentFactory.GetPaymentService(paymentMethodString);

            // Step 5: Gọi cổng thanh toán
            _logger.LogPaymentActivity(
                orderId: order.Id,
                paymentMethod: paymentMethodString,
                status: "PROCESSING");

            var paymentResult = await paymentService.ProcessPaymentAsync(
                order.TotalAmount,
                order.OrderNumber,
                $"Thanh toán đơn hàng {order.OrderNumber}");

            // Step 6: Xử lý kết quả
            if (paymentResult.IsSuccess)
            {
                // Đánh dấu đã thanh toán
                order.MarkAsPaid(paymentResult.TransactionId ?? Guid.NewGuid().ToString());
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: paymentMethodString,
                    status: "SUCCESS",
                    transactionId: paymentResult.TransactionId);
            }
            else
            {
                // Đánh dấu thanh toán thất bại
                order.MarkPaymentFailed(paymentResult.ErrorMessage);
                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();

                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: paymentMethodString,
                    status: "FAILED");
            }

            stopwatch.Stop();
            _logger.LogCommand(
                commandName: nameof(PayOrderCommand),
                commandId: command.CommandId,
                isSuccess: paymentResult.IsSuccess,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { OrderId = order.Id, TransactionId = paymentResult.TransactionId }
            );

            // Step 7: Trả về kết quả
            return CommandResult<PayOrderResult>.Success(new PayOrderResult
            {
                IsSuccess = paymentResult.IsSuccess,
                TransactionId = paymentResult.TransactionId,
                PaymentUrl = paymentResult.PaymentUrl,
                QrCode = paymentResult.QrCode,
                Amount = order.TotalAmount,
                PaymentMethod = command.PaymentMethod,
                Message = paymentResult.IsSuccess 
                    ? $"Thanh toán thành công cho đơn hàng {order.OrderNumber}"
                    : $"Thanh toán thất bại: {paymentResult.ErrorMessage}",
                ExpiresAt = paymentResult.PaymentUrl != null 
                    ? DateTime.UtcNow.AddMinutes(15) 
                    : null
            });
        }
        catch (ArgumentException ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Phương thức thanh toán không hợp lệ", ex);

            return CommandResult<PayOrderResult>.Failure(
                ex.Message,
                "INVALID_PAYMENT_METHOD");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"Lỗi thanh toán đơn hàng ID: {command.OrderId}", ex);
            _logger.LogCommand(
                commandName: nameof(PayOrderCommand),
                commandId: command.CommandId,
                isSuccess: false,
                executionTimeMs: stopwatch.ElapsedMilliseconds,
                data: new { Error = ex.Message }
            );

            return CommandResult<PayOrderResult>.Failure(
                "Có lỗi xảy ra khi thanh toán. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }
}


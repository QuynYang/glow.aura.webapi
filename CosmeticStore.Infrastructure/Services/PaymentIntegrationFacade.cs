using System.Diagnostics;
using CosmeticStore.Core.Enums;
using CosmeticStore.Core.Interfaces;
using CosmeticStore.Infrastructure.Gateways;

namespace CosmeticStore.Infrastructure.Services;


/// FACADE PATTERN - Đơn giản hóa quy trình thanh toán đơn hàng
/// ┌─────────────────────────────────────────────────────────┐
/// │           PaymentIntegrationFacade                       │
/// │  (Controller chỉ cần gọi 1 method duy nhất)            │
/// │                                                          │
/// │  ProcessPaymentAsync(orderId, paymentMethod) gọi:       │
/// │    ├── IOrderRepository    → Lấy đơn hàng từ DB        │
/// │    ├── PaymentGatewayFactory → Tạo cổng thanh toán     │
/// │    │   ├── MomoGateway     → Sinh QR + Deep Link       │
/// │    │   ├── VNPayGateway    → Sinh URL chuyển hướng     │
/// │    │   ├── ZaloPayGateway  → Sinh QR + URL             │
/// │    │   └── CODGateway      → Tạo giao dịch chờ        │
/// │    ├── Order.MarkAsPaid()  → Cập nhật trạng thái       │
/// │    └── IAppLogger          → Ghi log thanh toán        │
/// └─────────────────────────────────────────────────────────┘
public class PaymentIntegrationFacade : IPaymentIntegrationFacade
{
    private readonly IOrderRepository _orderRepository;
    private readonly PaymentGatewayFactory _paymentGatewayFactory;
    private readonly IAppLogger _logger;
    
    public PaymentIntegrationFacade(
        IOrderRepository orderRepository,
        PaymentGatewayFactory paymentGatewayFactory,
        IAppLogger logger)
    {
        _orderRepository = orderRepository;
        _paymentGatewayFactory = paymentGatewayFactory;
        _logger = logger;
    }

    
    /// Luồng xử lý:
    /// 1. Lấy đơn hàng từ DB
    /// 2. Kiểm tra trạng thái (State Pattern trong Order entity tự kiểm tra)
    /// 3. Xử lý COD (không cần thanh toán online)
    /// 4. Tạo cổng thanh toán từ Factory (Momo/VNPay/ZaloPay)
    /// 5. Gọi cổng thanh toán → Nhận QR code, URL chuyển hướng, Deep Link
    /// 6. Cập nhật đơn hàng (MarkAsPaid hoặc MarkPaymentFailed)
    /// 7. Lưu vào DB
    /// 8. Ghi log
    /// 9. Trả kết quả
    
    public async Task<PaymentFacadeResult> ProcessPaymentAsync(
        int orderId,
        PaymentMethod paymentMethod,
        string? returnUrl = null)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            
            // BƯỚC 1: Lấy đơn hàng từ DB
            
            var order = await _orderRepository.GetWithItemsAsync(orderId);

            if (order == null)
            {
                return PaymentFacadeResult.Fail(
                    "Không tìm thấy đơn hàng",
                    "ORDER_NOT_FOUND");
            }

            
            // BƯỚC 2: Kiểm tra trạng thái
            // STATE PATTERN: Order entity tự kiểm tra qua _currentState
            
            if (!order.Status.CanPay())
            {
                return PaymentFacadeResult.Fail(
                    $"Không thể thanh toán đơn hàng ở trạng thái '{order.Status.GetDescription()}'",
                    "INVALID_STATUS");
            }

            
            // BƯỚC 3: Xử lý COD (không cần thanh toán online)
            
            if (paymentMethod == PaymentMethod.COD)
            {
                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: "COD",
                    status: "PENDING_DELIVERY");

                return new PaymentFacadeResult
                {
                    IsSuccess = true,
                    Amount = order.TotalAmount,
                    PaymentMethod = PaymentMethod.COD,
                    Message = "Đơn hàng sẽ được thanh toán khi nhận hàng."
                };
            }

            
            // BƯỚC 4: Tạo cổng thanh toán từ Factory
            // Factory Pattern: PaymentGatewayFactory tạo đúng loại Gateway
            
            var gatewayCode = paymentMethod switch
            {
                PaymentMethod.Momo => "MOMO",
                PaymentMethod.VNPay => "VNPAY",
                PaymentMethod.ZaloPay => "ZALOPAY",
                PaymentMethod.BankTransfer => "BANK",
                _ => throw new ArgumentException($"Phương thức thanh toán không được hỗ trợ: {paymentMethod}")
            };

            var gateway = _paymentGatewayFactory.CreateGateway(gatewayCode);

            
            // BƯỚC 5: Gọi cổng thanh toán
            // Gateway tự xử lý: sinh QR code, URL chuyển hướng, Deep Link
            
            _logger.LogPaymentActivity(
                orderId: order.Id,
                paymentMethod: gatewayCode,
                status: "PROCESSING");

            var paymentRequest = new PaymentRequest
            {
                OrderId = order.Id.ToString(),
                OrderNumber = order.OrderNumber,
                Amount = order.TotalAmount,
                Description = $"Thanh toán đơn hàng {order.OrderNumber}",
                ReturnUrl = returnUrl
            };

            var paymentResponse = await gateway.ProcessPaymentAsync(paymentRequest);

            
            // BƯỚC 6: Cập nhật đơn hàng dựa trên kết quả
            // STATE PATTERN: Order.MarkAsPaid() tự delegate cho _currentState
            
            if (paymentResponse.IsSuccess)
            {
                var transactionId = paymentResponse.TransactionId ?? Guid.NewGuid().ToString();
                order.MarkAsPaid(transactionId);

                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: gatewayCode,
                    status: "SUCCESS",
                    transactionId: transactionId);
            }
            else
            {
                order.MarkPaymentFailed(paymentResponse.RawResponse);

                _logger.LogPaymentActivity(
                    orderId: order.Id,
                    paymentMethod: gatewayCode,
                    status: "FAILED");
            }

            
            // BƯỚC 7: Lưu vào DB
            
            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

            
            // BƯỚC 8: Ghi log tổng hợp
            
            stopwatch.Stop();
            _logger.LogInfo($"[FACADE] Thanh toán đơn #{order.OrderNumber} " +
                           $"qua {gatewayCode}: {(paymentResponse.IsSuccess ? "Thành công" : "Thất bại")} " +
                           $"({stopwatch.ElapsedMilliseconds}ms)");

            
            // BƯỚC 9: Trả kết quả
            
            return new PaymentFacadeResult
            {
                IsSuccess = paymentResponse.IsSuccess,
                Message = paymentResponse.IsSuccess
                    ? $"Thanh toán thành công cho đơn hàng {order.OrderNumber}"
                    : $"Thanh toán thất bại. Vui lòng thử lại.",
                TransactionId = paymentResponse.TransactionId,
                PaymentUrl = paymentResponse.PaymentUrl,
                QrCodeData = paymentResponse.QrCodeData,
                Amount = order.TotalAmount,
                PaymentMethod = paymentMethod,
                ExpiresAt = paymentResponse.PaymentUrl != null
                    ? DateTime.UtcNow.AddMinutes(15)
                    : null
            };
        }
        catch (InvalidOperationException ex)
        {
            // STATE PATTERN: Lỗi từ State object khi chuyển trạng thái không hợp lệ
            _logger.LogError($"[FACADE] Lỗi trạng thái khi thanh toán đơn ID: {orderId}", ex);
            return PaymentFacadeResult.Fail(ex.Message, "STATE_ERROR");
        }
        catch (ArgumentException ex)
        {
            _logger.LogError($"[FACADE] Phương thức thanh toán không hợp lệ", ex);
            return PaymentFacadeResult.Fail(ex.Message, "INVALID_PAYMENT_METHOD");
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError($"[FACADE] Lỗi thanh toán đơn ID: {orderId}", ex);
            return PaymentFacadeResult.Fail(
                "Có lỗi xảy ra khi thanh toán. Vui lòng thử lại.",
                "INTERNAL_ERROR");
        }
    }
}

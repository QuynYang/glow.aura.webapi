using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers.Notifications;

/// <summary>
/// Email Notification Handler - OBSERVER PATTERN
/// 
/// Lắng nghe các events và gửi email thông báo cho khách hàng.
/// 
/// Events được handle:
/// - OrderCreatedEvent → Email xác nhận đơn hàng
/// - OrderConfirmedEvent → Email xác nhận đơn hàng đã được xử lý
/// - PaymentSuccessEvent → Email xác nhận thanh toán
/// - OrderDeliveredEvent → Email xác nhận đã giao hàng
/// </summary>
public class OrderCreatedEmailHandler : IDomainEventHandler<OrderCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "OrderCreatedEmailHandler";
    public int Priority => 10; // Email gửi sớm

    public OrderCreatedEmailHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var subject = $"✅ Xác nhận đơn hàng #{domainEvent.OrderNumber}";
        var body = BuildOrderCreatedEmail(domainEvent);

        await _notificationService.SendEmailAsync(domainEvent.UserEmail, subject, body);

        _logger.LogInfo($"Order created email sent to {domainEvent.UserEmail}", new
        {
            domainEvent.OrderId,
            domainEvent.OrderNumber
        });
    }

    private static string BuildOrderCreatedEmail(OrderCreatedEvent e)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .order-info {{ background: white; padding: 20px; border-radius: 8px; margin: 20px 0; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
        .highlight {{ color: #667eea; font-weight: bold; }}
        .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1>🎉 Đặt hàng thành công!</h1>
        </div>
        <div class='content'>
            <p>Xin chào <strong>{e.UserName}</strong>,</p>
            <p>Cảm ơn bạn đã đặt hàng tại <strong>CosmeticStore</strong>! Đơn hàng của bạn đã được tiếp nhận và đang được xử lý.</p>
            
            <div class='order-info'>
                <h3>📦 Thông tin đơn hàng</h3>
                <p><strong>Mã đơn hàng:</strong> <span class='highlight'>#{e.OrderNumber}</span></p>
                <p><strong>Số sản phẩm:</strong> {e.ItemCount} sản phẩm</p>
                <p><strong>Tổng tiền:</strong> <span class='highlight'>{e.TotalAmount:N0} VND</span></p>
                <p><strong>Phương thức thanh toán:</strong> {e.PaymentMethod}</p>
                <p><strong>Địa chỉ giao hàng:</strong> {e.ShippingAddress}</p>
            </div>
            
            <p>Chúng tôi sẽ thông báo cho bạn khi đơn hàng được xác nhận và giao đi.</p>
            <p>Nếu có bất kỳ câu hỏi nào, vui lòng liên hệ hotline: <strong>1900-xxxx</strong></p>
            
            <div class='footer'>
                <p>💄 CosmeticStore - Đẹp tự nhiên, tự tin tỏa sáng!</p>
                <p>Email này được gửi tự động, vui lòng không trả lời.</p>
            </div>
        </div>
    </div>
</body>
</html>";
    }
}

/// <summary>
/// Handler gửi email khi thanh toán thành công
/// </summary>
public class PaymentSuccessEmailHandler : IDomainEventHandler<PaymentSuccessEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "PaymentSuccessEmailHandler";
    public int Priority => 15;

    public PaymentSuccessEmailHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentSuccessEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var subject = $"💳 Thanh toán thành công - Đơn hàng #{domainEvent.OrderNumber}";
        var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2 style='color: #28a745;'>✅ Thanh toán thành công!</h2>
    <p>Đơn hàng <strong>#{domainEvent.OrderNumber}</strong> đã được thanh toán thành công.</p>
    <ul>
        <li><strong>Phương thức:</strong> {domainEvent.PaymentMethod}</li>
        <li><strong>Số tiền:</strong> {domainEvent.Amount:N0} VND</li>
        <li><strong>Mã giao dịch:</strong> {domainEvent.TransactionId}</li>
    </ul>
    <p>Cảm ơn bạn đã mua hàng tại CosmeticStore!</p>
</body>
</html>";

        await _notificationService.SendEmailAsync(domainEvent.UserEmail, subject, body);

        _logger.LogInfo($"Payment success email sent to {domainEvent.UserEmail}");
    }
}

/// <summary>
/// Handler gửi email khi đơn hàng bị hủy
/// </summary>
public class OrderCancelledEmailHandler : IDomainEventHandler<OrderCancelledEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "OrderCancelledEmailHandler";
    public int Priority => 10;

    public OrderCancelledEmailHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(OrderCancelledEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var subject = $"❌ Đơn hàng #{domainEvent.OrderNumber} đã bị hủy";
        var body = $@"
<!DOCTYPE html>
<html>
<body style='font-family: Arial, sans-serif; padding: 20px;'>
    <h2 style='color: #dc3545;'>Đơn hàng đã bị hủy</h2>
    <p>Đơn hàng <strong>#{domainEvent.OrderNumber}</strong> đã được hủy.</p>
    <p><strong>Lý do:</strong> {domainEvent.CancellationReason}</p>
    {(domainEvent.RefundAmount > 0 ? $"<p><strong>Số tiền hoàn:</strong> {domainEvent.RefundAmount:N0} VND</p>" : "")}
    <p>Nếu bạn không yêu cầu hủy đơn, vui lòng liên hệ hotline: <strong>1900-xxxx</strong></p>
</body>
</html>";

        await _notificationService.SendEmailAsync(domainEvent.UserEmail, subject, body);

        _logger.LogInfo($"Order cancelled email sent to {domainEvent.UserEmail}");
    }
}


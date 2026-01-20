using CosmeticStore.Core.Events;
using CosmeticStore.Core.Interfaces;

namespace CosmeticStore.Infrastructure.Handlers.Notifications;

/// <summary>
/// Admin Alert Handler - OBSERVER PATTERN
/// 
/// Lắng nghe các events quan trọng và gửi thông báo cho Admin.
/// 
/// Events được handle:
/// - ReviewCreatedEvent → Thông báo có review mới
/// - ReviewReportedEvent → Cảnh báo review bị báo cáo
/// - ProductExpiringSoonEvent → Cảnh báo sản phẩm sắp hết hạn
/// - ProductLowStockEvent → Cảnh báo sản phẩm sắp hết hàng
/// - PaymentFailedEvent → Cảnh báo thanh toán thất bại
/// </summary>
public class ReviewCreatedAdminHandler : IDomainEventHandler<ReviewCreatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "ReviewCreatedAdminHandler";
    public int Priority => 50; // Admin alerts không cần quá sớm

    public ReviewCreatedAdminHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(ReviewCreatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var priority = domainEvent.Rating <= 2 ? AlertPriority.High : AlertPriority.Normal;
        
        var title = $"📝 Review mới cho sản phẩm: {domainEvent.ProductName}";
        var message = $@"
Người dùng: {domainEvent.UserName}
Đánh giá: {new string('⭐', domainEvent.Rating)}
Nội dung: {(string.IsNullOrEmpty(domainEvent.Content) ? "(Không có nội dung)" : domainEvent.Content)}
Có media: {(domainEvent.HasMedia ? "Có" : "Không")}";

        await _notificationService.SendAdminAlertAsync(title, message, priority);

        _logger.LogInfo($"Admin notified about new review for product {domainEvent.ProductId}");
    }
}

/// <summary>
/// Handler thông báo admin khi review bị report
/// </summary>
public class ReviewReportedAdminHandler : IDomainEventHandler<ReviewReportedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "ReviewReportedAdminHandler";
    public int Priority => 10; // Urgent

    public ReviewReportedAdminHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(ReviewReportedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var title = $"🚨 Review bị báo cáo - ID: {domainEvent.ReviewId}";
        var message = $@"
Review ID: {domainEvent.ReviewId}
Product ID: {domainEvent.ProductId}
Người báo cáo: User #{domainEvent.ReportedByUserId}
Lý do: {domainEvent.ReportReason}

Vui lòng kiểm tra và xử lý!";

        await _notificationService.SendAdminAlertAsync(title, message, AlertPriority.High);

        _logger.LogWarning($"Admin alerted: Review {domainEvent.ReviewId} was reported");
    }
}

/// <summary>
/// Handler thông báo admin khi sản phẩm sắp hết hạn
/// </summary>
public class ProductExpiringSoonAdminHandler : IDomainEventHandler<ProductExpiringSoonEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "ProductExpiringSoonAdminHandler";
    public int Priority => 30;

    public ProductExpiringSoonAdminHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(ProductExpiringSoonEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var priority = domainEvent.DaysUntilExpiry <= 7 ? AlertPriority.High : AlertPriority.Normal;

        var title = $"⏰ Sản phẩm sắp hết hạn: {domainEvent.ProductName}";
        var message = $@"
Sản phẩm: {domainEvent.ProductName}
Hạn sử dụng: {domainEvent.ExpiryDate:dd/MM/yyyy}
Còn lại: {domainEvent.DaysUntilExpiry} ngày
Tồn kho: {domainEvent.CurrentStock} sản phẩm
Giá hiện tại: {domainEvent.CurrentPrice:N0} VND

Đề xuất: Kích hoạt Flash Sale để giảm hàng tồn!";

        await _notificationService.SendAdminAlertAsync(title, message, priority);

        _logger.LogInfo($"Admin alerted: Product {domainEvent.ProductId} expiring in {domainEvent.DaysUntilExpiry} days");
    }
}

/// <summary>
/// Handler thông báo admin khi sản phẩm sắp hết hàng
/// </summary>
public class ProductLowStockAdminHandler : IDomainEventHandler<ProductLowStockEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "ProductLowStockAdminHandler";
    public int Priority => 20;

    public ProductLowStockAdminHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(ProductLowStockEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var priority = domainEvent.CurrentStock <= 5 ? AlertPriority.High : AlertPriority.Normal;

        var title = $"📦 Sản phẩm sắp hết hàng: {domainEvent.ProductName}";
        var message = $@"
Sản phẩm: {domainEvent.ProductName}
Tồn kho hiện tại: {domainEvent.CurrentStock}
Ngưỡng cảnh báo: {domainEvent.Threshold}

Vui lòng đặt hàng nhập kho!";

        await _notificationService.SendAdminAlertAsync(title, message, priority);

        _logger.LogInfo($"Admin alerted: Product {domainEvent.ProductId} low stock ({domainEvent.CurrentStock} remaining)");
    }
}

/// <summary>
/// Handler thông báo admin khi thanh toán thất bại
/// </summary>
public class PaymentFailedAdminHandler : IDomainEventHandler<PaymentFailedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "PaymentFailedAdminHandler";
    public int Priority => 10; // Urgent

    public PaymentFailedAdminHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(PaymentFailedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var title = $"💳 Thanh toán thất bại - Đơn hàng #{domainEvent.OrderNumber}";
        var message = $@"
Đơn hàng: #{domainEvent.OrderNumber}
User ID: {domainEvent.UserId}
Phương thức: {domainEvent.PaymentMethod}
Lỗi: {domainEvent.ErrorMessage}

Cần theo dõi và hỗ trợ khách hàng!";

        await _notificationService.SendAdminAlertAsync(title, message, AlertPriority.High);

        _logger.LogWarning($"Admin alerted: Payment failed for Order #{domainEvent.OrderNumber}");
    }
}

/// <summary>
/// Handler thông báo Flash Sale cho users có skin type phù hợp
/// </summary>
public class FlashSaleNotificationHandler : IDomainEventHandler<FlashSaleActivatedEvent>
{
    private readonly INotificationService _notificationService;
    private readonly ISystemLogger _logger;

    public string HandlerName => "FlashSaleNotificationHandler";
    public int Priority => 40;

    public FlashSaleNotificationHandler(INotificationService notificationService, ISystemLogger logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task HandleAsync(FlashSaleActivatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        // Thông báo admin
        var title = $"⚡ Flash Sale đã kích hoạt: {domainEvent.ProductName}";
        var message = $@"
Sản phẩm: {domainEvent.ProductName}
Giá gốc: {domainEvent.OriginalPrice:N0} VND
Giảm: {domainEvent.DiscountPercent}%
Giá sale: {domainEvent.SalePrice:N0} VND
Thời gian: {domainEvent.StartTime:HH:mm dd/MM} - {domainEvent.EndTime:HH:mm dd/MM}";

        await _notificationService.SendAdminAlertAsync(title, message, AlertPriority.Normal);

        _logger.LogInfo($"Flash sale notification sent for product {domainEvent.ProductId}");

        // TODO: Gửi push notification cho users quan tâm sản phẩm này
    }
}


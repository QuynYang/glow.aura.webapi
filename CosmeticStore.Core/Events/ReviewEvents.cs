namespace CosmeticStore.Core.Events;

/// <summary>
/// Event khi có review mới được tạo
/// 
/// OBSERVER PATTERN:
/// - Raised by: CreateReviewCommandHandler
/// - Handled by: AdminAlertHandler, ProductRatingUpdateHandler
/// </summary>
public class ReviewCreatedEvent : DomainEventBase
{
    public int ReviewId { get; }
    public int ProductId { get; }
    public string ProductName { get; }
    public int UserId { get; }
    public string UserName { get; }
    public int Rating { get; }
    public string? Content { get; }
    public bool HasMedia { get; }
    public List<string>? MediaUrls { get; }

    public ReviewCreatedEvent(
        int reviewId,
        int productId,
        string productName,
        int userId,
        string userName,
        int rating,
        string? content,
        bool hasMedia,
        List<string>? mediaUrls = null)
    {
        ReviewId = reviewId;
        ProductId = productId;
        ProductName = productName;
        UserId = userId;
        UserName = userName;
        Rating = rating;
        Content = content;
        HasMedia = hasMedia;
        MediaUrls = mediaUrls;
    }
}

/// <summary>
/// Event khi review bị report
/// </summary>
public class ReviewReportedEvent : DomainEventBase
{
    public int ReviewId { get; }
    public int ProductId { get; }
    public int ReportedByUserId { get; }
    public string ReportReason { get; }

    public ReviewReportedEvent(
        int reviewId,
        int productId,
        int reportedByUserId,
        string reportReason)
    {
        ReviewId = reviewId;
        ProductId = productId;
        ReportedByUserId = reportedByUserId;
        ReportReason = reportReason;
    }
}

/// <summary>
/// Event khi review được duyệt
/// </summary>
public class ReviewApprovedEvent : DomainEventBase
{
    public int ReviewId { get; }
    public int ProductId { get; }
    public int UserId { get; }
    public string UserEmail { get; }

    public ReviewApprovedEvent(
        int reviewId,
        int productId,
        int userId,
        string userEmail)
    {
        ReviewId = reviewId;
        ProductId = productId;
        UserId = userId;
        UserEmail = userEmail;
    }
}


using CosmeticStore.Core.Commands.Orders;

namespace CosmeticStore.Infrastructure.Handlers.Validations.Orders;

public class CouponValidator : OrderValidationHandler
{
    private static readonly Dictionary<string, DateTime> CouponExpirations = new(StringComparer.OrdinalIgnoreCase)
    {
        { "WELCOME10", new DateTime(2027, 12, 31, 23, 59, 59, DateTimeKind.Utc) },
        { "SALE20", new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc) },
        { "GIAM50K", new DateTime(2026, 10, 31, 23, 59, 59, DateTimeKind.Utc) },
        { "VIP30", new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc) },
        { "FREESHIP", new DateTime(2027, 6, 30, 23, 59, 59, DateTimeKind.Utc) }
    };

    protected override Task<OrderValidationResult> ValidateCurrentAsync(OrderValidationContext context, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(context.Command.CouponCode))
        {
            return Task.FromResult(OrderValidationResult.Success());
        }

        var couponCode = context.Command.CouponCode.Trim();

        if (!CouponExpirations.TryGetValue(couponCode, out var expiredAt))
        {
            return Task.FromResult(OrderValidationResult.Failure("Mã giảm giá không hợp lệ", "INVALID_COUPON"));
        }

        if (DateTime.UtcNow > expiredAt)
        {
            return Task.FromResult(OrderValidationResult.Failure("Mã giảm giá đã hết hạn", "COUPON_EXPIRED"));
        }

        return Task.FromResult(OrderValidationResult.Success());
    }
}

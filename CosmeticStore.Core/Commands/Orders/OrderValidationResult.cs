namespace CosmeticStore.Core.Commands.Orders;

public class OrderValidationResult
{
    public bool IsValid { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? ErrorCode { get; private set; }
    public Dictionary<string, string[]>? ValidationErrors { get; private set; }

    public static OrderValidationResult Success() => new()
    {
        IsValid = true
    };

    public static OrderValidationResult Failure(string errorMessage, string? errorCode = null) => new()
    {
        IsValid = false,
        ErrorMessage = errorMessage,
        ErrorCode = errorCode
    };

    public static OrderValidationResult ValidationFailure(Dictionary<string, string[]> errors) => new()
    {
        IsValid = false,
        ErrorMessage = "Dữ liệu đơn hàng không hợp lệ",
        ErrorCode = "VALIDATION_ERROR",
        ValidationErrors = errors
    };
}

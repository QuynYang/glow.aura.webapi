namespace CosmeticStore.Core.Commands;

/// <summary>
/// Interface cho Command Handler
/// 
/// COMMAND PATTERN - HANDLER:
/// - Mỗi Command có một Handler tương ứng
/// - Handler chứa logic xử lý
/// - Tách biệt hoàn toàn với Command (data)
/// 
/// Lợi ích:
/// - Single Responsibility: Command chứa data, Handler chứa logic
/// - Open/Closed: Thêm Command mới không sửa code cũ
/// - Testable: Dễ mock và test từng handler
/// </summary>
/// <typeparam name="TCommand">Loại Command</typeparam>
/// <typeparam name="TResult">Kiểu dữ liệu trả về</typeparam>
public interface ICommandHandler<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Xử lý Command
    /// </summary>
    /// <param name="command">Command cần xử lý</param>
    /// <param name="cancellationToken">Token hủy</param>
    /// <returns>Kết quả xử lý</returns>
    Task<CommandResult<TResult>> HandleAsync(TCommand command, CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface cho Handler không trả về dữ liệu
/// </summary>
/// <typeparam name="TCommand">Loại Command</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit> where TCommand : ICommand
{
}

/// <summary>
/// Kết quả thực thi Command
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
public class CommandResult<T>
{
    /// <summary>
    /// Thành công hay không
    /// </summary>
    public bool IsSuccess { get; protected set; }

    /// <summary>
    /// Dữ liệu trả về (nếu thành công)
    /// </summary>
    public T? Data { get; protected set; }

    /// <summary>
    /// Thông báo lỗi (nếu thất bại)
    /// </summary>
    public string? ErrorMessage { get; protected set; }

    /// <summary>
    /// Mã lỗi (nếu thất bại)
    /// </summary>
    public string? ErrorCode { get; protected set; }

    /// <summary>
    /// Danh sách lỗi validation
    /// </summary>
    public Dictionary<string, string[]>? ValidationErrors { get; protected set; }

    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static CommandResult<T> Success(T data)
    {
        return new CommandResult<T>
        {
            IsSuccess = true,
            Data = data
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại
    /// </summary>
    public static CommandResult<T> Failure(string errorMessage, string? errorCode = null)
    {
        return new CommandResult<T>
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }

    /// <summary>
    /// Tạo kết quả lỗi validation
    /// </summary>
    public static CommandResult<T> ValidationFailure(Dictionary<string, string[]> errors)
    {
        return new CommandResult<T>
        {
            IsSuccess = false,
            ErrorCode = "VALIDATION_ERROR",
            ErrorMessage = "Dữ liệu không hợp lệ",
            ValidationErrors = errors
        };
    }
}

/// <summary>
/// Kết quả Command không trả về dữ liệu
/// </summary>
public class CommandResult : CommandResult<Unit>
{
    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static CommandResult Success()
    {
        return new CommandResult
        {
            IsSuccess = true,
            Data = Unit.Value
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại
    /// </summary>
    public new static CommandResult Failure(string errorMessage, string? errorCode = null)
    {
        return new CommandResult
        {
            IsSuccess = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode
        };
    }
}


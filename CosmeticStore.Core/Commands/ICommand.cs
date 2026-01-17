namespace CosmeticStore.Core.Commands;

/// <summary>
/// Interface cơ bản cho tất cả Command
/// 
/// COMMAND PATTERN:
/// - Đóng gói request thành object
/// - Tách biệt "yêu cầu" và "xử lý"
/// - Cho phép: queue, log, undo/redo, validate
/// 
/// Workflow:
/// 1. Client tạo Command object với dữ liệu cần thiết
/// 2. Command được gửi đến Handler tương ứng
/// 3. Handler thực thi và trả về Result
/// </summary>
/// <typeparam name="TResult">Kiểu dữ liệu trả về</typeparam>
public interface ICommand<TResult>
{
    /// <summary>
    /// ID duy nhất của Command (để tracking/logging)
    /// </summary>
    Guid CommandId { get; }

    /// <summary>
    /// Thời điểm tạo Command
    /// </summary>
    DateTime Timestamp { get; }
}

/// <summary>
/// Interface cho Command không trả về dữ liệu
/// </summary>
public interface ICommand : ICommand<Unit>
{
}

/// <summary>
/// Unit type - Đại diện cho "void" trong generic
/// </summary>
public readonly struct Unit : IEquatable<Unit>
{
    public static readonly Unit Value = new();

    public bool Equals(Unit other) => true;
    public override bool Equals(object? obj) => obj is Unit;
    public override int GetHashCode() => 0;
    public override string ToString() => "()";

    public static bool operator ==(Unit left, Unit right) => true;
    public static bool operator !=(Unit left, Unit right) => false;
}

/// <summary>
/// Base class cho tất cả Command
/// Cung cấp CommandId và Timestamp mặc định
/// </summary>
/// <typeparam name="TResult">Kiểu dữ liệu trả về</typeparam>
public abstract class CommandBase<TResult> : ICommand<TResult>
{
    public Guid CommandId { get; } = Guid.NewGuid();
    public DateTime Timestamp { get; } = DateTime.UtcNow;
}

/// <summary>
/// Base class cho Command không trả về dữ liệu
/// </summary>
public abstract class CommandBase : CommandBase<Unit>
{
}


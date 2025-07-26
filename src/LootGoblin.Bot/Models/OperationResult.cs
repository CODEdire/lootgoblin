namespace LootGoblin.Bot.Models;

/// <summary>
/// Represents the result of an operation, including status, message, and value.
/// </summary>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public record OperationResult<T>(
    OperationStatus Status,
    string? Message = null,
    T? Value = default
)
{
    /// <summary>
    /// Gets a value indicating whether the operation was successful.
    /// </summary>
    public bool IsSuccess => Status == OperationStatus.Success;

    /// <summary>
    /// Creates a successful <see cref="OperationResult{T}"/>.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A successful operation result.</returns>
    public static OperationResult<T> Success(T value) =>
        new(OperationStatus.Success, null, value);

    /// <summary>
    /// Creates a failed <see cref="OperationResult{T}"/>.
    /// </summary>
    /// <param name="status">The status of the failed operation.</param>
    /// <param name="message">The error message.</param>
    /// <returns>A failed operation result.</returns>
    public static OperationResult<T> Fail(OperationStatus status, string message) =>
        new(status, message);
}
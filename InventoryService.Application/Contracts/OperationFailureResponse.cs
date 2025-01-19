namespace InventoryService.Application.Contracts;
/// <summary>
/// Represents a response indicating a failure in an operation.
/// </summary>
public record OperationFailureResponse
{
    /// <summary>
    /// Gets the collection of errors that occurred during the operation.
    /// </summary>
    public required IEnumerable<OperationResponse> Errors { get; init; }
}

public record OperationResponse
{
    public required string Message { get; init; }
}
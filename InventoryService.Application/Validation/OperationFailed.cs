namespace InventoryService.Application.Validation;
/// <summary>
/// Represents a failure in an operation, containing one or more error messages.
/// </summary>
/// <param name="Messages">An array of error messages describing the failure.</param>
public record OperationFailed(string[] Messages)
{
    public OperationFailed(string message) : this([message])
    {
    }
}
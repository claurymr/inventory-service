namespace InventoryService.Application.Validation;
public record OperationFailed(string[] Messages)
{
    public OperationFailed(string message) : this([message])
    {
    }
}
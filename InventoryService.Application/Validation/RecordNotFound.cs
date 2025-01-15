namespace InventoryService.Application.Validation;
public record RecordNotFound(string[] Messages)
{
    public RecordNotFound(string message) : this([message])
    {
    }
}
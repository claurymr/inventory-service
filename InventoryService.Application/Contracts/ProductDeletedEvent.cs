namespace InventoryService.Application.Contracts;

public record ProductDeletedEvent
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
}

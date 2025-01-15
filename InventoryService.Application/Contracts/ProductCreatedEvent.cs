namespace InventoryService.Application.Contracts;
public record ProductCreatedEvent
{
    public Guid Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
}

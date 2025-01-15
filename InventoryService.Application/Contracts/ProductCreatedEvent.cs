namespace InventoryService.Application.Contracts;
public record ProductCreatedEvent
{
    public Guid Id { get; set; }
    public decimal Price { get; init; }
}

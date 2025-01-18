namespace Shared.Contracts.Events;
public record InventoryAdjustedEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public int OldQuantity { get; set; }
    public int NewQuantity { get; init; }
}
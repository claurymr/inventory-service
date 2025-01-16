using InventoryService.Application.Contracts;

namespace InventoryService.Application.Inventories.AdjustInventories;
public record InventoryAdjustedEvent
{
    public Guid ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public string Action { get; init; }
    public int OldQuantity { get; set; }
    public int NewQuantity { get; init; }
}
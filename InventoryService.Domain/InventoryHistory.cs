namespace InventoryService.Domain;

public class InventoryHistory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public Guid InventoryId { get; set; }
    public decimal OldQuantity { get; set; }
    public decimal NewQuantity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Inventory Inventory { get; set; }
}

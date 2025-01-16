namespace InventoryService.Domain;

public class InventoryHistory
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid InventoryId { get; set; }
    public int OldQuantity { get; set; }
    public int NewQuantity { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public Inventory Inventory { get; set; }
}

namespace InventoryService.Domain;

/// <summary>
/// Represents the history of inventory changes.
/// </summary>
public class InventoryHistory
{
    /// <summary>
    /// Gets or sets the unique identifier for the inventory history record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the inventory.
    /// </summary>
    public Guid InventoryId { get; set; }

    /// <summary>
    /// Gets or sets the old quantity of the product before the change.
    /// </summary>
    public int OldQuantity { get; set; }

    /// <summary>
    /// Gets or sets the new quantity of the product after the change.
    /// </summary>
    public int NewQuantity { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the change occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Gets or sets the inventory associated with this history record.
    /// </summary>
    public Inventory Inventory { get; set; }
}

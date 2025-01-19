namespace InventoryService.Domain;

/// <summary>
/// Represents an inventory item.
/// </summary>
public class Inventory
{
    /// <summary>
    /// Gets or sets the unique identifier for the inventory item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the product.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the name of the product.
    /// </summary>
    public string ProductName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product in the inventory.
    /// </summary>
    public int Quantity { get; set; }
}
namespace Shared.Contracts.Events;

/// <summary>
/// Represents an event that occurs when the inventory is adjusted.
/// </summary>
public record InventoryAdjustedEvent
{
    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the action performed on the inventory (e.g., "Added", "Removed").
    /// </summary>
    public string Action { get; init; } = string.Empty;

    /// <summary>
    /// Gets or sets the old quantity of the product before the adjustment.
    /// </summary>
    public int OldQuantity { get; set; }

    /// <summary>
    /// Gets the new quantity of the product after the adjustment.
    /// </summary>
    public int NewQuantity { get; init; }
}
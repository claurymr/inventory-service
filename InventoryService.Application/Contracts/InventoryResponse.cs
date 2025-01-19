namespace InventoryService.Application.Contracts;

/// <summary>
/// Represents a response containing inventory details.
/// </summary>
public record InventoryResponse
{
    /// <summary>
    /// Gets the unique identifier for the inventory item.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier for the product.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the quantity of the product in inventory.
    /// </summary>
    public int Quantity { get; init; }
}

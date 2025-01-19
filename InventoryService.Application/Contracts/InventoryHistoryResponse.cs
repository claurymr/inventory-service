namespace InventoryService.Application.Contracts;
/// <summary>
/// Represents the response for inventory history.
/// </summary>
public record InventoryHistoryResponse
{
    /// <summary>
    /// Gets the unique identifier for the inventory history record.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the unique identifier for the product.
    /// </summary>
    public Guid ProductId { get; init; }

    /// <summary>
    /// Gets the unique identifier for the inventory.
    /// </summary>
    public Guid InventoryId { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;

    /// <summary>
    /// Gets the old quantity of the product before the change.
    /// </summary>
    public decimal OldQuantity { get; init; }

    /// <summary>
    /// Gets the new quantity of the product after the change.
    /// </summary>
    public decimal NewQuantity { get; init; }

    /// <summary>
    /// Gets the timestamp when the change occurred.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; }
}
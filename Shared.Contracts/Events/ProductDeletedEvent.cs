namespace Shared.Contracts.Events;

/// <summary>
/// Represents an event that occurs when a product is deleted.
/// </summary>
public record ProductDeletedEvent
{
    /// <summary>
    /// Gets the unique identifier of the product.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Gets the name of the product.
    /// </summary>
    public string ProductName { get; init; } = string.Empty;
}

namespace Shared.Contracts.Events;
/// <summary>
/// Represents an event that is triggered when a product is created.
/// </summary>
public record ProductCreatedEvent
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

namespace Shared.Contracts.Events;
/// <summary>
/// Event that is triggered when a product is updated.
/// </summary>
public record ProductUpdatedEvent
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
namespace Shared.Contracts.Events;

public record ProductDeletedEvent
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
}

namespace Shared.Contracts.Events;
public record ProductCreatedEvent
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
}

namespace Shared.Contracts.Events;
public record ProductUpdatedEvent
{
    public Guid Id { get; init; }
    public string ProductName { get; init; } = string.Empty;
}
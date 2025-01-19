using InventoryService.Application.EventBus;
using MassTransit;

namespace InventoryService.Infrastructure.MessageBroker;
/// <summary>
/// Represents an event bus that is responsible for publishing events.
/// </summary>
/// <param name="publishEndpoint">The endpoint used to publish events.</param>
public sealed class EventBus(IPublishEndpoint publishEndpoint) : IEventBus
{
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    /// <inheritdoc />
    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        where TEvent : class =>
        _publishEndpoint.Publish(@event, cancellationToken);

}
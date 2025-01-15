using InventoryService.Application.Contracts;
using MassTransit;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductCreatedConsumer : IConsumer<ProductCreatedEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        // Log the event
        // Add to inventory and inventory history
        throw new NotImplementedException();
    }
}

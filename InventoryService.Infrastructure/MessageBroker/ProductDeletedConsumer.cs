using InventoryService.Application.Contracts;
using MassTransit;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductDeletedConsumer : IConsumer<ProductDeletedEvent>
{
    public Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        // Log the event
        // update to 0 from inventory and inventory history
        throw new NotImplementedException();
    }
}
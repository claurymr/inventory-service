using InventoryService.Application.Contracts;
using MassTransit;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductUpdatedConsumer : IConsumer<ProductUpdatedEvent>
{
    public Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        throw new NotImplementedException();
    }
}
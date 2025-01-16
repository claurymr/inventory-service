using InventoryService.Application.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductUpdatedConsumer(ILogger<ProductCreatedConsumer> logger) : IConsumer<ProductUpdatedEvent>
{
    private readonly ILogger<ProductCreatedConsumer> _logger = logger;

    public Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        // Log the event
        _logger.LogInformation(
                "ProductUpdatedEvent consumed with ProductId: {Id} - Name: {Name}",
                context.Message.Id,
                context.Message.ProductName);

        return Task.CompletedTask;
    }
}
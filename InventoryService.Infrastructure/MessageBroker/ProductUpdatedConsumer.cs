using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductUpdatedConsumer(IInventoryRepository inventoryRepository, ILogger<ProductCreatedConsumer> logger)
: IConsumer<ProductUpdatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly ILogger<ProductCreatedConsumer> _logger = logger;

    public Task Consume(ConsumeContext<ProductUpdatedEvent> context)
    {
        // Log the event
        _logger.LogInformation(
                "ProductUpdatedEvent consumed with ProductId: {Id} - Name: {Name}",
                context.Message.Id,
                context.Message.ProductName);
        var inventory = new Inventory
        {
            ProductId = context.Message.Id,
            ProductName = context.Message.ProductName
        };

        _inventoryRepository.UpdateProductInInventoryByIdAsync(context.Message.Id, inventory);
        return Task.CompletedTask;
    }
}
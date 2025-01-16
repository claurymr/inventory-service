using InventoryService.Application.Contracts;
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductCreatedConsumer(IInventoryRepository inventoryRepository, ILogger<ProductCreatedConsumer> logger)
    : IConsumer<ProductCreatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly ILogger<ProductCreatedConsumer> _logger = logger;

    public Task Consume(ConsumeContext<ProductCreatedEvent> context)
    {
        // Log the event
        _logger.LogInformation(
                "ProductCreatedEvent consumed with ProductId: {Id} - Name: {Name}",
                context.Message.Id,
                context.Message.ProductName);

        // Add to inventory and inventory history
        var inventory = new Inventory
        {
            ProductId = context.Message.Id,
            ProductName = context.Message.ProductName,
            Quantity = 0
        };
        return _inventoryRepository.CreateInventoryAsync(inventory);
    }
}

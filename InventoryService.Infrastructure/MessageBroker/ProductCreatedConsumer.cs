using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductCreatedConsumer
    (IInventoryRepository inventoryRepository,
    IInventoryHistoryRepository inventoryHistoryRepository,
    ILogger<ProductCreatedConsumer> logger)
    : IConsumer<ProductCreatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;
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
            Quantity = default
        };

        _inventoryRepository.CreateInventoryAsync(inventory);
        AddInventoryHistory(inventory);
        return Task.CompletedTask;
    }

    private Task AddInventoryHistory(Inventory inventory)
    {
        var inventoryHistory = new InventoryHistory
        {
            ProductId = inventory.ProductId,
            InventoryId = inventory.Id,
            OldQuantity = default,
            NewQuantity = inventory.Quantity,
            Timestamp = DateTime.UtcNow
        };

        return _inventoryHistoryRepository.CreateInventoryHistoryAsync(inventoryHistory);
    }
}

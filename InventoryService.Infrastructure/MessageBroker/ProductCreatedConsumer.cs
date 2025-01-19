using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

/// <summary>
/// Consumer for handling ProductCreatedEvent messages.
/// </summary>
/// <param name="inventoryRepository">The inventory repository.</param>
/// <param name="inventoryHistoryRepository">The inventory history repository.</param>
/// <param name="logger">The logger instance.</param>
///
/// <remarks>
/// This consumer listens for ProductCreatedEvent messages and processes them by:
/// 1. Logging the event details.
/// 2. Adding the product to the inventory.
/// 3. Adding an entry to the inventory history.
/// </remarks>
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

    /// <summary>
    /// Adds an entry to the inventory history.
    /// </summary>
    /// <param name="inventory">The inventory to add to the history.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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

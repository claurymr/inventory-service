using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

public sealed class ProductDeletedConsumer
    (IInventoryRepository inventoryRepository,
    ILogger<ProductDeletedEvent> logger,
    IInventoryHistoryRepository inventoryHistoryRepository)
    : IConsumer<ProductDeletedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;
    private readonly ILogger<ProductDeletedEvent> _logger = logger;
    public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
    {
        // Log the event
        _logger.LogInformation(
                "ProductDeletedEvent consumed with ProductId: {Id} - Name: {Name}",
                context.Message.Id,
                context.Message.ProductName);

        var result = await _inventoryRepository.UpdateInventoryToInitialAsync(context.Message.Id);
        var inventoryHistory = new InventoryHistory
        {
            ProductId = context.Message.Id,
            InventoryId = result.Inventory.Id,
            OldQuantity = result.OldQuantity,
            NewQuantity = default,
            Timestamp = DateTime.UtcNow
        };
        await _inventoryHistoryRepository.CreateInventoryHistoryAsync(inventoryHistory);
    }
}
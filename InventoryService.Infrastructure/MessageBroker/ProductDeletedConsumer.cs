using InventoryService.Application.Contracts;
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;

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

        var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(context.Message.Id);
        var result = await _inventoryRepository.UpdateInventoryToInitialAsync(context.Message.Id, 0);

        await _inventoryHistoryRepository
            .CreateInventoryHistoryAsync(
                new InventoryHistory
                {
                    ProductId = context.Message.Id,
                    InventoryId = inventory.Id,
                    OldQuantity = result.OldQuantity,
                    NewQuantity = 0
                });
    }
}
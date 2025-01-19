using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

/// <summary>
/// Consumer class for handling ProductDeletedEvent messages.
/// </summary>
/// <param name="inventoryRepository">The inventory repository to update inventory data.</param>
/// <param name="logger">The logger to log information about the event.</param>
/// <param name="inventoryHistoryRepository">The inventory history repository to create inventory history records.</param>
///
/// <remarks>
/// This consumer listens for ProductDeletedEvent messages and processes them by:
/// 1. Updating the inventory to its initial state.
/// 2. Logging the event.
/// 3. Creates a history record of the inventory change.
/// </remarks>
public sealed class ProductDeletedConsumer
    (IInventoryRepository inventoryRepository,
    ILogger<ProductDeletedEvent> logger,
    IInventoryHistoryRepository inventoryHistoryRepository)
    : IConsumer<ProductDeletedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;
    private readonly ILogger<ProductDeletedEvent> _logger = logger;
   
   /// <inheritdoc/>
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
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.MessageBroker;

/// <summary>
/// Consumer class for handling <see cref="ProductUpdatedEvent"/> messages.
/// </summary>
/// <param name="inventoryRepository">The inventory repository to update product information.</param>
/// <param name="logger">The logger to log information and errors.</param>
///
/// <remarks>
/// This consumer listens for <see cref="ProductUpdatedEvent"/> messages and updates the product information
/// in the inventory repository. It logs the event details when a message is consumed.
/// </remarks>
public sealed class ProductUpdatedConsumer(IInventoryRepository inventoryRepository, ILogger<ProductCreatedConsumer> logger)
: IConsumer<ProductUpdatedEvent>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly ILogger<ProductCreatedConsumer> _logger = logger;

    /// <inheritdoc/>
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
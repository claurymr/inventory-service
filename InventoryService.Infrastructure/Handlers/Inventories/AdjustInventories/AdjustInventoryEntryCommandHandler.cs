using FluentValidation;
using MediatR;
using InventoryService.Application.Repositories;
using InventoryService.Application.Inventories.AdjustInventories;
using InventoryService.Application.EventBus;
using InventoryService.Application.Validation;
using InventoryService.Application.Contracts;
using InventoryService.Domain;
using Shared.Contracts.Events;

namespace InventoryService.Infrastructure.Handlers.Inventories.AdjustInventories;
/// <summary>
/// Handles the adjustment of inventory entries.
/// </summary>
/// <param name="inventoryRepository">The inventory repository.</param>
/// <param name="inventoryHistoryRepository">The inventory history repository.</param>
/// <param name="validator">The validator for the AdjustInventoryEntryCommand.</param>
/// <param name="eventBus">The event bus for publishing events.</param>
/// <returns>
/// A result containing the product ID if successful, or a validation failure or record not found error.
/// </returns>
public class AdjustInventoryEntryCommandHandler
    (IInventoryRepository inventoryRepository,
    IInventoryHistoryRepository inventoryHistoryRepository,
    IValidator<AdjustInventoryEntryCommand> validator,
    IEventBus eventBus)
    : IRequestHandler<AdjustInventoryEntryCommand, ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;
    private readonly IValidator<AdjustInventoryEntryCommand> _validator = validator;
    private readonly IEventBus _eventBus = eventBus;

    public async Task<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
        Handle(AdjustInventoryEntryCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return new ValidationFailed(validationResult.Errors);
        }

        (Inventory Inventory, int OldQuantity) = await _inventoryRepository
                        .AdjustInventoryAsync(request.ProductId, (Domain.Enums.ActionType)request.Action, request.Quantity);

        if (Inventory is null)
        {
            return new RecordNotFound($"Inventory with Product Id {request.ProductId} not found.");
        }

        // Add action to inventory history
        await AddInventoryHistory(Inventory, OldQuantity);

        // Publish the product created event to the message broker.
        await _eventBus.PublishAsync(
                new InventoryAdjustedEvent
                {
                    ProductId = request.ProductId,
                    Action = request.Action.ToString(),
                    ProductName = Inventory.ProductName,
                    OldQuantity = OldQuantity,
                    NewQuantity = Inventory.Quantity
                }, cancellationToken);

        return Inventory.ProductId;
    }

    private async Task AddInventoryHistory(Inventory inventory, int oldQuantity)
    {
        var inventoryHistory = new InventoryHistory
        {
            InventoryId = inventory.Id,
            ProductId = inventory.ProductId,
            OldQuantity = oldQuantity,
            NewQuantity = inventory.Quantity,
            Timestamp = DateTime.UtcNow
        };
        await _inventoryHistoryRepository.CreateInventoryHistoryAsync(inventoryHistory);
    }
}

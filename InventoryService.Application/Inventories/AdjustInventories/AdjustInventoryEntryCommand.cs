using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.Inventories.AdjustInventories;

/// <summary>
/// Represents a command to adjust the inventory entry for a specific product.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Quantity">The quantity to adjust in the inventory.</param>
/// <returns>A result containing the unique identifier of the adjustment, or validation errors, or a record not found error.</returns>
public record AdjustInventoryEntryCommand(
    Guid ProductId,
    int Quantity)
    : IRequest<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    public ActionType Action { get; private init; } = ActionType.Entry;
}

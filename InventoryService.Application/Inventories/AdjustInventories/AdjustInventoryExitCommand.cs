using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.Inventories.AdjustInventories;
/// <summary>
/// Command to adjust the inventory by exiting a specified quantity of a product.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Quantity">The quantity of the product to be exited from the inventory.</param>
/// <returns>A result containing the unique identifier of the adjustment, or validation errors, or a record not found error.</returns>
public record AdjustInventoryExitCommand(
    Guid ProductId,
    int Quantity)
    : IRequest<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    public ActionType Action { get; private init; } = ActionType.Exit;
}


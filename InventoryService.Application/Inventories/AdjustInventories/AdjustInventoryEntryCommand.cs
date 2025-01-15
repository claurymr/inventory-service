using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.Inventories.AdjustInventories;
public record AdjustInventoryEntryCommand(
    Guid ProductId,
    int Quantity)
    : IRequest<ResultWithWarning<Guid, ValidationFailed, RecordNotFound>>
{
    public ActionType Action { get; private init; } = ActionType.Entry;
}

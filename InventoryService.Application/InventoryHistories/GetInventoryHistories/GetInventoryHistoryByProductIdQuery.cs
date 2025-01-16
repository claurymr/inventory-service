using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.InventoryHistories.GetInventoryHistories;
public record GetInventoryHistoryByProductIdQuery(Guid ProductId)
    : IRequest<Result<IEnumerable<InventoryHistoryResponse>, OperationFailed>>;

using InventoryService.Application.Contracts;
using MediatR;

namespace InventoryService.Application.InventoryHistories.GetInventoryHistories;
public record GetInventoryHistoryByProductIdQuery(Guid ProductId)
    : IRequest<IEnumerable<InventoryHistoryResponse>>;

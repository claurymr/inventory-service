using MediatR;
using InventoryService.Application.Repositories;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using InventoryService.Application.Contracts;
using InventoryService.Application.Mappings;

namespace InventoryService.Infrastructure.Handlers.InventoryHistories.GetInventoryHistories;
public class GetInventoryHistoryByProductIdQueryHandler(IInventoryHistoryRepository inventoryHistoryRepository)
    : IRequestHandler<GetInventoryHistoryByProductIdQuery, IEnumerable<InventoryHistoryResponse>>
{
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;

    public async Task<IEnumerable<InventoryHistoryResponse>>
        Handle(GetInventoryHistoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        var inventoryHistories = await _inventoryHistoryRepository.GetInventoryHistoryByProductIdAsync(request.ProductId);
        return inventoryHistories.MapToResponse();
    }
}
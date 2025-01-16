using MediatR;
using InventoryService.Application.Repositories;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using InventoryService.Application.Contracts;
using InventoryService.Application.Mappings;
using InventoryService.Application.Validation;

namespace InventoryService.Infrastructure.Handlers.InventoryHistories.GetInventoryHistories;
public class GetInventoryHistoryByProductIdQueryHandler(IInventoryHistoryRepository inventoryHistoryRepository)
    : IRequestHandler<GetInventoryHistoryByProductIdQuery, Result<IEnumerable<InventoryHistoryResponse>, OperationFailed>>
{
    private readonly IInventoryHistoryRepository _inventoryHistoryRepository = inventoryHistoryRepository;

    public async Task<Result<IEnumerable<InventoryHistoryResponse>, OperationFailed>>
        Handle(GetInventoryHistoryByProductIdQuery request, CancellationToken cancellationToken)
    {

        IEnumerable<InventoryHistoryResponse> inventoryHistoriesResponse;
        try
        {
            var inventoryHistories = await _inventoryHistoryRepository.GetInventoryHistoryByProductIdAsync(request.ProductId);
            inventoryHistoriesResponse = inventoryHistories.MapToResponse();
        }
        catch (Exception ex)
        {
            return new OperationFailed(ex.Message);
        }

        return inventoryHistoriesResponse.ToList();
    }
}
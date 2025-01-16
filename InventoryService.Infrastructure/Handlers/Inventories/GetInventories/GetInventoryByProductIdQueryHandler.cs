using MediatR;
using ProductService.Application.Contracts;
using InventoryService.Application.Repositories;
using InventoryService.Application.Inventories.GetInventories;
using InventoryService.Application.Validation;
using InventoryService.Application.Contracts;
using InventoryService.Application.Mappings;

namespace InventoryService.Infrastructure.Handlers.Inventories.GetInventories;
public class GetInventoryByProductIdQueryHandler(IInventoryRepository inventoryRepository)
    : IRequestHandler<GetInventoryByProductIdQuery, Result<InventoryResponse, RecordNotFound>>
{
    private readonly IInventoryRepository _inventoryRepository = inventoryRepository;

    public async Task<Result<InventoryResponse, RecordNotFound>>
        Handle(GetInventoryByProductIdQuery request, CancellationToken cancellationToken)
    {
        var inventory = await _inventoryRepository.GetInventoryByProductIdAsync(request.Id);
        if (inventory is null)
        {
            return new RecordNotFound([$"Inventory with Product Id {request.Id} not found."]);
        }
        return inventory.MapToResponse();
    }
}
using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;
using ProductService.Application.Contracts;

namespace InventoryService.Application.Inventories.GetInventories;
public record GetInventoryByProductIdQuery(Guid Id)
    : IRequest<Result<InventoryResponse, RecordNotFound>>;
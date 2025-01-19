using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.Inventories.GetInventories;
/// <summary>
/// Query to get the inventory details by product ID.
/// </summary>
/// <param name="Id">The unique identifier of the product.</param>
/// <returns>A result containing the inventory response if found, otherwise a record not found error.</returns>
public record GetInventoryByProductIdQuery(Guid Id)
    : IRequest<Result<InventoryResponse, RecordNotFound>>;
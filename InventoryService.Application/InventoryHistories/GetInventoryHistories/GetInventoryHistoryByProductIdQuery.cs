using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using MediatR;

namespace InventoryService.Application.InventoryHistories.GetInventoryHistories;
/// <summary>
/// Query to get the inventory history by product ID.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <returns>A result containing an enumerable of <see cref="InventoryHistoryResponse"/> if successful, or an <see cref="OperationFailed"/> error if the operation fails.</returns>
public record GetInventoryHistoryByProductIdQuery(Guid ProductId)
    : IRequest<Result<IEnumerable<InventoryHistoryResponse>, OperationFailed>>;

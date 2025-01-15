using FluentValidation.Results;
using InventoryService.Application.Contracts;
using InventoryService.Application.Validation;
using InventoryService.Domain;
using ProductService.Application.Contracts;

namespace InventoryService.Application.Mappings;
public static class ContractMapping
{

    public static ValidationFailureResponse MapToResponse(this IEnumerable<ValidationFailure> validationFailures)
    {
        return new ValidationFailureResponse
        {
            Errors = validationFailures.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage
            })
        };
    }

    public static ValidationFailureResponse MapToResponse(this ValidationFailed failed)
    {
        return new ValidationFailureResponse
        {
            Errors = failed.Errors.Select(x => new ValidationResponse
            {
                PropertyName = x.PropertyName,
                Message = x.ErrorMessage
            })
        };
    }

    public static InventoryResponse MapToResponse(this Inventory inventory)
    {
        return new InventoryResponse
        {
            Id = inventory.Id,
            ProductId = inventory.ProductId,
            ProductName = inventory.ProductName,
            Quantity = inventory.Quantity
        };
    }

    public static IEnumerable<InventoryResponse> MapToResponse(this IEnumerable<Inventory> inventories)
    {
        return inventories.Select(MapToResponse);
    }

    public static InventoryHistoryResponse MapToResponse(this InventoryHistory inventoryHistory)
    {
        return new InventoryHistoryResponse
        {
            Id = inventoryHistory.Id,
            ProductId = inventoryHistory.ProductId,
            ProductName = inventoryHistory.Inventory.ProductName,
            OldQuantity = inventoryHistory.OldQuantity,
            NewQuantity = inventoryHistory.NewQuantity,
            Timestamp = inventoryHistory.Timestamp
        };
    }

    public static IEnumerable<InventoryHistoryResponse> MapToResponse(this IEnumerable<InventoryHistory> inventoryHistories)
    {
        return inventoryHistories.Select(MapToResponse);
    }
}
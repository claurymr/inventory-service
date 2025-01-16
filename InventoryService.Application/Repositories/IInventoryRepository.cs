using InventoryService.Domain;
using InventoryService.Domain.Enums;

namespace InventoryService.Application.Repositories;
public interface IInventoryRepository
{
    Task<Guid> CreateInventoryAsync(Inventory inventory);
    Task<Inventory> GetInventoryByProductIdAsync(Guid id);
    Task<(Inventory Inventory, int OldQuantity)> AdjustInventoryAsync(Guid id, ActionType actionType, int quantity);
}
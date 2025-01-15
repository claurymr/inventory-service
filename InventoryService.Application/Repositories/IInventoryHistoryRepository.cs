using InventoryService.Domain;

namespace InventoryService.Application.Repositories;
public interface IInventoryHistoryRepository
{
    Task<IEnumerable<InventoryHistory>> GetInventoryHistoryByProductIdAsync(Guid id);
    Task CreateInventoryHistoryAsync(InventoryHistory inventoryHistory);
}
using InventoryService.Domain;

namespace InventoryService.Application.Repositories;
public interface IInventoryRepository
{
    Task<Guid> CreateInventoryAsync(Inventory inventory);
    Task<Inventory> GetInventoryByIdAsync(Guid id);
    Task<Guid> UpdateInventoryAsync(Guid id, Inventory inventory);
}
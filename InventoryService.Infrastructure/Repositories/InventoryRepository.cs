using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Infrastructure.Repositories;
public class InventoryRepository(InventoryServiceDbContext dbContext) : IInventoryRepository
{
    private readonly InventoryServiceDbContext _dbContext = dbContext;

    public async Task<Guid> CreateInventoryAsync(Inventory inventory)
    {
        await _dbContext.Inventories.AddAsync(inventory);
        await _dbContext.SaveChangesAsync();
        return inventory.Id;
    }

    public async Task<Inventory> GetInventoryByIdAsync(Guid id) => (await _dbContext.Inventories.FindAsync(id))!;

    public async Task<Guid> UpdateInventoryAsync(Guid id, Inventory inventory)
    {
        var existingInventory = await _dbContext.Inventories.FindAsync(id);
        if (existingInventory == null)
        {
            return Guid.Empty;
        }

        existingInventory.ProductId = id;
        existingInventory.Quantity = inventory.Quantity;
        await _dbContext.SaveChangesAsync();

        return existingInventory.Id;
    }
}
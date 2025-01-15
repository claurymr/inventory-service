using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Domain.Enums;
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

    public async Task<Inventory> GetInventoryByProductIdAsync(Guid id) => (await _dbContext.Inventories.FindAsync(id))!;

    public async Task<(Inventory Inventory, int OldQuantity)> AdjustInventoryAsync(Guid id, ActionType action, int quantity)
    {
        var existingInventory = await _dbContext.Inventories.FindAsync(id);
        if (existingInventory == null)
        {
            return (existingInventory, default)!;
        }
        var oldQut = existingInventory.Quantity;
        existingInventory.ProductId = id;

        if (action == ActionType.Entry)
        {
            existingInventory.Quantity += quantity;
        }
        else if (action == ActionType.Exit)
        {
            existingInventory.Quantity -= quantity;
        }

        await _dbContext.SaveChangesAsync();
        return (existingInventory, oldQut);
    }
}
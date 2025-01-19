using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Domain.Enums;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

/// <summary>
/// Repository class for managing inventory operations.
/// </summary>
/// <param name="dbContext">The database context for inventory service.</param>
public class InventoryRepository(InventoryServiceDbContext dbContext) : IInventoryRepository
{
    private readonly InventoryServiceDbContext _dbContext = dbContext;

    /// <inheritdoc/>
    public async Task<Guid> CreateInventoryAsync(Inventory inventory)
    {
        await _dbContext.Inventories.AddAsync(inventory);
        await _dbContext.SaveChangesAsync();
        return inventory.Id;
    }

    /// <inheritdoc/>
    public async Task<Inventory> GetInventoryByProductIdAsync(Guid id)
    {
        return (await _dbContext.Inventories
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ProductId == id))!;
    }

    /// <inheritdoc/>
    public async Task<(Inventory Inventory, int OldQuantity)> AdjustInventoryAsync(Guid id, ActionType action, int quantity)
    {
        var existingInventory = (await _dbContext.Inventories.FirstOrDefaultAsync(x => x.ProductId == id))!;
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

        existingInventory.Quantity =
            existingInventory.Quantity < 0
            ? default
            : existingInventory.Quantity;

        await _dbContext.SaveChangesAsync();
        return (existingInventory, oldQut);
    }

    /// <inheritdoc/>
    public async Task<(Inventory Inventory, int OldQuantity)> UpdateInventoryToInitialAsync(Guid id)
    {
        var existingInventory = (await _dbContext.Inventories.FirstOrDefaultAsync(x => x.ProductId == id))!;
        if (existingInventory == null)
        {
            return (existingInventory, default)!;
        }
        var oldQut = existingInventory.Quantity;
        existingInventory.Quantity = default;

        await _dbContext.SaveChangesAsync();
        return (existingInventory, oldQut);
    }

    /// <inheritdoc/>
    public async Task UpdateProductInInventoryByIdAsync(Guid id, Inventory inventory)
    {
        var existingInventory = (await _dbContext.Inventories.FirstOrDefaultAsync(x => x.ProductId == id))!;
        if (existingInventory == null)
        {
            return;
        }
        existingInventory.ProductName = inventory.ProductName;

        await _dbContext.SaveChangesAsync();
    }
}
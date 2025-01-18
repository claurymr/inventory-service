using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;
public sealed class InventoryHistoryRepository(InventoryServiceDbContext dbContext) : IInventoryHistoryRepository
{
    private readonly InventoryServiceDbContext _dbContext = dbContext;

    public async Task CreateInventoryHistoryAsync(InventoryHistory inventoryHistory)
    {
        await _dbContext.InventoryHistories.AddAsync(inventoryHistory);
        await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<InventoryHistory>> GetInventoryHistoryByProductIdAsync(Guid id)
    {
        return await _dbContext.InventoryHistories
                        .AsNoTracking()
                        .Where(x => x.ProductId == id)
                        .Include(x => x.Inventory)
                        .ToListAsync();
    }
}
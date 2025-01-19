using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Repositories;

/// <summary>
/// Repository class for managing inventory history data in the database.
/// </summary>
/// <param name="dbContext">The database context used for accessing inventory history data.</param>
public sealed class InventoryHistoryRepository(InventoryServiceDbContext dbContext) : IInventoryHistoryRepository
{
    private readonly InventoryServiceDbContext _dbContext = dbContext;

    /// <inheritdoc />
    public async Task CreateInventoryHistoryAsync(InventoryHistory inventoryHistory)
    {
        await _dbContext.InventoryHistories.AddAsync(inventoryHistory);
        await _dbContext.SaveChangesAsync();
    }

    /// <inheritdoc />
    public async Task<IEnumerable<InventoryHistory>> GetInventoryHistoryByProductIdAsync(Guid id)
    {
        return await _dbContext.InventoryHistories
                        .AsNoTracking()
                        .Where(x => x.ProductId == id)
                        .Include(x => x.Inventory)
                        .ToListAsync();
    }
}
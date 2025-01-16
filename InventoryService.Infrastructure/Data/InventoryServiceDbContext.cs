using InventoryService.Domain;
using Microsoft.EntityFrameworkCore;

namespace InventoryService.Infrastructure.Data;

public class InventoryServiceDbContext(DbContextOptions<InventoryServiceDbContext> options) : DbContext(options)
{
    public DbSet<Inventory> Inventories { get; set; }
    public DbSet<InventoryHistory> InventoryHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryServiceDbContext).Assembly);
    }
}

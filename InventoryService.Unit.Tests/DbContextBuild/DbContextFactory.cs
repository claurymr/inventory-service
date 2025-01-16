using InventoryService.Infrastructure.Data;

namespace InventoryService.Unit.Tests.DbContextBuild;
public class DbContextFactory
{
    public static InventoryServiceDbContext CreateInMemoryDbContext()
    {
        return new TestDbContext().Context;
    }
}
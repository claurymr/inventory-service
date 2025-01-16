using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace InventoryService.Infrastructure.Extensions;
public static class HostExtensions
{
    public static IHost MigrateDatabase(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<InventoryServiceDbContext>();

        try
        {
            // Apply pending migrations
            dbContext.Database.Migrate();
            //Ensure the database is created
            dbContext.Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<InventoryServiceDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
        }

        return host;
    }
}
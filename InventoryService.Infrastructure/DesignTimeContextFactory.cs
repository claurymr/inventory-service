using InventoryService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace InventoryService.Infrastructure;
public class DesignTimeContextFactory : IDesignTimeDbContextFactory<InventoryServiceDbContext>
{
    public InventoryServiceDbContext CreateDbContext(string[] args)
    {
        var apiProjectDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "InventoryService.Api");
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configuration = new ConfigurationBuilder()
                .SetBasePath(apiProjectDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
                .Build();

        var connectionString = configuration.GetConnectionString("InventoryServiceConnection");
        var optionsBuilder = new DbContextOptionsBuilder<InventoryServiceDbContext>();
        optionsBuilder.UseSqlite(connectionString);

        return new InventoryServiceDbContext(optionsBuilder.Options);
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using InventoryService.Infrastructure.Data;

namespace InventoryService.Unit.Tests.DbContextBuild;

public class TestDbContext : IDisposable
{
    private readonly SqliteConnection _connection;
    public InventoryServiceDbContext Context { get; }

    public TestDbContext()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<InventoryServiceDbContext>()
            .UseSqlite(_connection)
            .Options;

        Context = new InventoryServiceDbContext(options);
        Context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        Context?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}
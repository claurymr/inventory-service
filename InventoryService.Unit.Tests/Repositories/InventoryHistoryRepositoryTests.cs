using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using InventoryService.Domain;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Unit.Tests.DbContextBuild;

namespace InventoryService.Unit.Tests.Repositories;
public class InventoryHistoryRepositoryTests
{
    private readonly IFixture _fixture;
    private readonly InventoryServiceDbContext _dbContextMock;

    public InventoryHistoryRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _dbContextMock = DbContextFactory.CreateInMemoryDbContext();
    }

    [Fact]
    public async Task GetInventoryHistoryByProductIdAsync_ShouldReturnInventoryHistory_WhenProductIdExists()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryHistoryRepository(_dbContextMock);

        var expectedInventory = _fixture
                                .Build<Inventory>()
                                .With(p => p.ProductId, productAdd)
                                .Create();
        var expectedHistories = _fixture
                                .Build<InventoryHistory>()
                                .With(p => p.ProductId, productAdd)
                                .With(p => p.InventoryId, expectedInventory.Id)
                                .With(p => p.Timestamp, DateTime.UtcNow.AddDays(-1))
                                .CreateMany(2)
                                .ToList();

        await _dbContextMock.Inventories.AddAsync(expectedInventory);
        await _dbContextMock.InventoryHistories.AddRangeAsync(expectedHistories);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var histories = await repository.GetInventoryHistoryByProductIdAsync(productAdd);

        // Assert
        histories.Should().HaveCount(2);
        histories.Should().AllSatisfy(c => c.ProductId = productAdd);
    }

    [Fact]
    public async Task GetInventoryHistoryByProductIdAsync_ShouldReturnEmpty_WhenProductIdDoesNotExist()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryHistoryRepository(_dbContextMock);

        var expectedInventory = _fixture
                                .Build<Inventory>()
                                .With(p => p.ProductId, productAdd)
                                .Create();
        var expectedHistories = _fixture
                                .Build<InventoryHistory>()
                                .With(p => p.ProductId, productAdd)
                                .With(p => p.InventoryId, expectedInventory.Id)
                                .With(p => p.Timestamp, DateTime.UtcNow.AddDays(-1))
                                .CreateMany(2)
                                .ToList();

        await _dbContextMock.Inventories.AddAsync(expectedInventory);
        await _dbContextMock.InventoryHistories.AddRangeAsync(expectedHistories);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var histories = await repository.GetInventoryHistoryByProductIdAsync(Guid.NewGuid());

        // Assert
        histories.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateInventoryHistoryAsync_ShouldCreateInventoryHistory()
    {
        // Arrange
        var repository = new InventoryHistoryRepository(_dbContextMock);

        var expectedInventory = _fixture
                                .Build<Inventory>()
                                .With(p => p.ProductId, Guid.NewGuid())
                                .Create();
        await _dbContextMock.Inventories.AddAsync(expectedInventory);
        await _dbContextMock.SaveChangesAsync();

        var expectedHistory = _fixture
                                .Build<InventoryHistory>()
                                .With(p => p.ProductId, expectedInventory.ProductId)
                                .With(p => p.InventoryId, expectedInventory.Id)
                                .With(p => p.Timestamp, DateTime.UtcNow.AddDays(-1))
                                .Create();

        // Act
        await repository.CreateInventoryHistoryAsync(expectedHistory);

        // Assert
        var history = await _dbContextMock.InventoryHistories.FindAsync(expectedHistory.Id);
        history.Should().NotBeNull();
        history.Should().BeEquivalentTo(expectedHistory, options => options.Excluding(p => p.Id));
    }
}
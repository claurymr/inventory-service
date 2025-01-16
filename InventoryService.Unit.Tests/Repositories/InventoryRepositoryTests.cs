using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using InventoryService.Domain;
using InventoryService.Domain.Enums;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Unit.Tests.DbContextBuild;

namespace InventoryService.Unit.Tests.Repositories;
public class InventoryRepositoryTests
{
    private readonly IFixture _fixture;
    private readonly InventoryServiceDbContext _dbContextMock;

    public InventoryRepositoryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _dbContextMock = DbContextFactory.CreateInMemoryDbContext();
    }

    [Fact]
    public async Task CreateInventoryAsync_ShouldAddInventoryToDatabase()
    {
        // Arrange
        var inventory = _fixture
                        .Build<Inventory>()
                        .With(p => p.Id, Guid.Empty)
                        .Create();
        var repository = new InventoryRepository(_dbContextMock);

        // Act
        await repository.CreateInventoryAsync(inventory);

        // Assert
        _dbContextMock.Inventories.Should().HaveCount(1);
        _dbContextMock.Inventories.First().Should().BeEquivalentTo(inventory, options => options.Excluding(p => p.Id));
    }

    [Fact]
    public async Task GetInventoryByIdAsync_ShouldReturnInventoryByProductId_WhenProductIdExists()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryRepository(_dbContextMock);

        var expectedInventory = _fixture
                                .Build<Inventory>()
                                .Without(p => p.Id)
                                .With(p => p.ProductId, productAdd)
                                .Create();
        await _dbContextMock.Inventories.AddAsync(expectedInventory);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var inventory = await repository.GetInventoryByProductIdAsync(productAdd);

        // Assert
        inventory.Should().NotBeNull();
        inventory.ProductId.Should().Be(productAdd);
    }

    [Fact]
    public async Task GetInventoryByIdAsync_ShouldReturnDefault_WhenProductDoesNotExist()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryRepository(_dbContextMock);

        // Act
        var inventory = await repository.GetInventoryByProductIdAsync(productAdd);

        // Assert
        inventory.Should().BeNull();
    }

    [Fact]
    public async Task AdjustInventoryAsync_ShouldAdjustInventory_WhenInventoryExists()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryRepository(_dbContextMock);

        var expectedInventory = _fixture
                                .Build<Inventory>()
                                .Without(p => p.Id)
                                .With(p => p.ProductId, productAdd)
                                .With(p => p.Quantity, 10)
                                .Create();
        await _dbContextMock.Inventories.AddAsync(expectedInventory);
        await _dbContextMock.SaveChangesAsync();

        // Act
        var (inventory, oldQuantity) = await repository.AdjustInventoryAsync(productAdd, ActionType.Entry, 5);

        // Assert
        inventory.Should().NotBeNull();
        inventory.ProductId.Should().Be(productAdd);
        inventory.Quantity.Should().Be(15);
        oldQuantity.Should().Be(10);
    }

    [Fact]
    public async Task AdjustInventoryAsync_ShouldReturnDefault_WhenInventoryDoesNotExist()
    {
        // Arrange
        var productAdd = Guid.NewGuid();
        var repository = new InventoryRepository(_dbContextMock);

        // Act
        var (inventory, oldQuantity) = await repository.AdjustInventoryAsync(productAdd, ActionType.Entry, 5);

        // Assert
        inventory.Should().BeNull();
        oldQuantity.Should().Be(default);
    }
}
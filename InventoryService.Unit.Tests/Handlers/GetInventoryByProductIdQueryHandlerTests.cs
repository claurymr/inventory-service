using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.GetInventories;
using InventoryService.Application.Mappings;
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Handlers.Inventories.GetInventories;
using Moq;

namespace InventoryService.Unit.Tests.Handlers;
public class GetInventoryByProductIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly GetInventoryByProductIdQueryHandler _handler;

    public GetInventoryByProductIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _inventoryRepositoryMock = _fixture.Freeze<Mock<IInventoryRepository>>();
        _handler = new GetInventoryByProductIdQueryHandler(_inventoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInventory_WhenInventoryWithProductIdExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var inventory = _fixture
                        .Build<Inventory>()
                        .With(p => p.ProductId, productId)
                        .Create();
        var query = new GetInventoryByProductIdQuery(productId);

        _inventoryRepositoryMock
            .Setup(repo => repo.GetInventoryByProductIdAsync(productId))
            .ReturnsAsync(inventory);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result
            .Match(
            inventoryResponse => inventoryResponse,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.ProductId.Should().Be(productId);

        _inventoryRepositoryMock.Verify(repo => repo.GetInventoryByProductIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnRecordNotFound_WhenInventoryWithProductIdNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryByProductIdQuery(productId);

        _inventoryRepositoryMock
            .Setup(repo => repo.GetInventoryByProductIdAsync(productId))
            .ReturnsAsync(default(Inventory)!);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            notFound => notFound.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<OperationFailureResponse>();
        resultError.Errors.Should().ContainSingle(e => e.Message == $"Inventory with Product Id {productId} not found.");

        _inventoryRepositoryMock.Verify(repo => repo.GetInventoryByProductIdAsync(productId), Times.Once);
    }
}
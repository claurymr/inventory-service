using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using InventoryService.Application.Contracts;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using InventoryService.Application.Mappings;
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Handlers.InventoryHistories.GetInventoryHistories;
using Moq;

namespace InventoryService.Unit.Tests.Handlers;
public class GetInventoryHistoryByProductIdQueryHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInventoryHistoryRepository> _inventoryHistoryRepositoryMock;
    private readonly GetInventoryHistoryByProductIdQueryHandler _handler;

    public GetInventoryHistoryByProductIdQueryHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _inventoryHistoryRepositoryMock = _fixture.Freeze<Mock<IInventoryHistoryRepository>>();
        _handler = new GetInventoryHistoryByProductIdQueryHandler(_inventoryHistoryRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnInventoryHistories_WhenProductIdIsProvidedAndExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var inventoryHistories = _fixture
                        .Build<InventoryHistory>()
                        .With(p => p.ProductId, productId)
                        .CreateMany(5)
                        .ToList();

        var query = new GetInventoryHistoryByProductIdQuery(productId);

        _inventoryHistoryRepositoryMock
            .Setup(repo => repo.GetInventoryHistoryByProductIdAsync(productId))
            .ReturnsAsync(inventoryHistories);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var resultValue = result
            .Match(
            inventoryResponses => inventoryResponses,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().HaveCount(inventoryHistories.Count);


        _inventoryHistoryRepositoryMock.Verify(repo => repo.GetInventoryHistoryByProductIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoInventoryHistoriesFoundForProductId()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryHistoryByProductIdQuery(productId);

        _inventoryHistoryRepositoryMock
            .Setup(repo => repo.GetInventoryHistoryByProductIdAsync(productId))
            .ReturnsAsync([]);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        var resultValue = result
            .Match(
            inventoryResponses => inventoryResponses,
            _ => default!);
        resultValue.Should().NotBeNull();
        resultValue.Should().BeEmpty();

        _inventoryHistoryRepositoryMock.Verify(repo => repo.GetInventoryHistoryByProductIdAsync(productId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOperationFailed_WhenExceptionThrown()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryHistoryByProductIdQuery(productId);

        _inventoryHistoryRepositoryMock
            .Setup(repo => repo.GetInventoryHistoryByProductIdAsync(productId))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            operationFailed => operationFailed.MapToResponse());
        resultError.Should().NotBeNull();
        resultError.Should().BeOfType<OperationFailureResponse>();

        _inventoryHistoryRepositoryMock.Verify(repo => repo.GetInventoryHistoryByProductIdAsync(productId), Times.Once);
    }
}
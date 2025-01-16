using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation.Results;
using InventoryService.Api.Endpoints.Inventories;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.AdjustInventories;
using InventoryService.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Application.Contracts;

namespace InventoryService.Unit.Tests.Endpoints.Inventories;
public class AdjustInventoryExitEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly AdjustInventoryExitEndpoint _endpoint;

    public AdjustInventoryExitEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new AdjustInventoryExitEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAdjustExitIventory_WhenIsUpdated()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 10;
        var inventory = _fixture
                        .Build<InventoryResponse>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Quantity, 100)
                        .Create();
        var expectedInventory = _fixture
                        .Build<InventoryResponse>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Quantity, inventory.Quantity + quantity)
                        .Create();
        var request = _fixture.Build<AdjustInventoryExitCommand>()
                              .With(c => c.ProductId, productId)
                              .With(c => c.Quantity, quantity)
                              .Create();
        var validationResult = new ValidationResult();
        _mediatorMock
            .Setup(mediator => mediator.Send(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedInventory.ProductId);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NoContent));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnBadRequest_WhenRequestIsInvalid()
    {
        // Arrange
        var request = _fixture
                        .Build<AdjustInventoryExitCommand>()
                        .Without(p => p.ProductId)
                        .Create();
        var validationFailed = new ValidationFailed(
                new ValidationFailure(
                    nameof(AdjustInventoryExitCommand.ProductId),
                    "ProductId is required"));

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<AdjustInventoryExitCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationFailed);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequest<ValidationFailureResponse>));
        (result.Result as BadRequest<ValidationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == "ProductId is required");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotFound_WhenProductIsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<AdjustInventoryExitCommand>()
                        .With(p => p.ProductId, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<AdjustInventoryExitCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecordNotFound([$"Inventory with ProductId {productId} not found."]));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFound<OperationFailureResponse>));
        (result.Result as NotFound<OperationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == $"Inventory with ProductId {productId} not found.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
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
public class AdjustInventoryEntryEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private AdjustInventoryEntryEndpoint? _endpoint;

    public AdjustInventoryEntryEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldAdjustEntryIventory_WhenIsUpdated()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var action = ActionType.Entry;
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
        var request = _fixture.Build<AdjustInventoryEntryCommand>()
                              .With(c => c.ProductId, productId)
                              .With(c => c.Quantity, quantity)
                              .Create();
        var validationResult = new ValidationResult();
        _endpoint = Factory.Create<AdjustInventoryEntryEndpoint>(
                c => c.Request.RouteValues.Add("productId", productId.ToString()),
                _mediatorMock.Object);
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
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<AdjustInventoryEntryCommand>()
                        .With(p => p.ProductId, productId)
                        .Without(p => p.Quantity)
                        .Create();
        var validationFailed = new ValidationFailed(
                new ValidationFailure(
                    nameof(AdjustInventoryEntryCommand.Quantity),
                    "Quantity is required"));
        _endpoint = Factory.Create<AdjustInventoryEntryEndpoint>(
                c => c.Request.RouteValues.Add("productId", productId.ToString()),
                _mediatorMock.Object);
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<AdjustInventoryEntryCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationFailed);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(BadRequest<ValidationFailureResponse>));
        (result.Result as BadRequest<ValidationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == "Quantity is required");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotFound_WhenProductIsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<AdjustInventoryEntryCommand>()
                        .With(p => p.ProductId, productId)
                        .Create();
        _endpoint = Factory.Create<AdjustInventoryEntryEndpoint>(
                c => c.Request.RouteValues.Add("productId", productId.ToString()),
                _mediatorMock.Object);
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<AdjustInventoryEntryCommand>(), It.IsAny<CancellationToken>()))
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
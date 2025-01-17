using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
using FluentAssertions;
using InventoryService.Api.Endpoints.Inventories;
using InventoryService.Application.Contracts;
using InventoryService.Application.Inventories.GetInventories;
using InventoryService.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Moq;
using ProductService.Application.Contracts;

namespace InventoryService.Unit.Tests.Endpoints.Inventories;
public class GetInventoryByProductIdEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private GetInventoryByProductIdEndpoint? _endpoint;

    public GetInventoryByProductIdEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnInventory_WhenProductIdExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var inventory = _fixture
                        .Build<InventoryResponse>()
                        .With(p => p.ProductId, productId)
                        .Create();
        var request = _fixture
                        .Build<GetInventoryByProductIdQuery>()
                        .With(p => p.Id, productId)
                        .Create();
        _endpoint = Factory.Create<GetInventoryByProductIdEndpoint>(
                c => c.Request.RouteValues.Add("productId", productId.ToString()),
                _mediatorMock.Object);
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetInventoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventory);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<InventoryResponse>));
        (result.Result as Ok<InventoryResponse>)!.Value.Should().BeEquivalentTo(inventory);

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldReturnNotFound_WhenProductIdDoNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetInventoryByProductIdQuery>()
                        .With(p => p.Id, productId)
                        .Create();
        _endpoint = Factory.Create<GetInventoryByProductIdEndpoint>(
                c => c.Request.RouteValues.Add("productId", productId.ToString()),
                _mediatorMock.Object);
        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetInventoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RecordNotFound([$"Inventory with Product Id {productId} not found."]));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(NotFound<OperationFailureResponse>));
        (result.Result as NotFound<OperationFailureResponse>)!.Value!
            .Errors.Should().Contain(c => c.Message == $"Inventory with Product Id {productId} not found.");

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
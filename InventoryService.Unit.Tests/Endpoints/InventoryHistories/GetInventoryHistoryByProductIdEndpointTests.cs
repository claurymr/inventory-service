using System.Reflection;
using AutoFixture;
using AutoFixture.AutoMoq;
using FastEndpoints;
using FluentAssertions;
using InventoryService.Api.Endpoints.InventoryHistories;
using InventoryService.Application.Contracts;
using InventoryService.Application.InventoryHistories.GetInventoryHistories;
using InventoryService.Application.Validation;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;

namespace InventoryService.Unit.Tests.Endpoints.InventoryHistories;
public class GetInventoryHistoryByProductIdEndpointTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly GetInventoryHistoryByProductIdEndpoint _endpoint;

    public GetInventoryHistoryByProductIdEndpointTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _mediatorMock = _fixture.Freeze<Mock<IMediator>>();
        _endpoint = new GetInventoryHistoryByProductIdEndpoint(_mediatorMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInventoryHistories_WhenProductIdExists()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var inventoryHistories = _fixture
                        .Build<InventoryHistoryResponse>()
                        .With(p => p.ProductId, productId)
                        .CreateMany(5)
                        .ToList();
        var request = _fixture
                        .Build<GetInventoryHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetInventoryHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(inventoryHistories);

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<InventoryHistoryResponse>>));
        (result.Result as Ok<IEnumerable<InventoryHistoryResponse>>)!.Value.Should().BeEquivalentTo(inventoryHistories);

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyList_WhenProductIdDoNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetInventoryHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetInventoryHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<InventoryHistoryResponse>());

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(Ok<IEnumerable<InventoryHistoryResponse>>));
        (result.Result as Ok<IEnumerable<InventoryHistoryResponse>>)!.Value.Should().BeEmpty();

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnInternalServerError_WhenExceptionIsThrown()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var request = _fixture
                        .Build<GetInventoryHistoryByProductIdQuery>()
                        .With(p => p.ProductId, productId)
                        .Create();

        _mediatorMock
            .Setup(mediator => mediator.Send(It.IsAny<GetInventoryHistoryByProductIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OperationFailed("An error occurred."));

        // Act
        var result = await _endpoint.ExecuteAsync(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType(typeof(JsonHttpResult<OperationFailureResponse>));

        _mediatorMock.Verify(mediator => mediator.Send(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}
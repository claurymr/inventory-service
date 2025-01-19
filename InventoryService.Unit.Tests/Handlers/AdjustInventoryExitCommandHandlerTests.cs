using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using InventoryService.Application.Contracts;
using InventoryService.Application.EventBus;
using InventoryService.Application.Inventories.AdjustInventories;
using InventoryService.Application.Repositories;
using InventoryService.Domain;
using InventoryService.Infrastructure.Handlers.Inventories.AdjustInventories;
using Moq;
using Shared.Contracts.Events;

namespace InventoryService.Unit.Tests.Handlers;
public class AdjustInventoryExitCommandHandlerTests
{
    private readonly IFixture _fixture;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly Mock<IInventoryHistoryRepository> _inventoryHistoryRepositoryMock;
    private readonly Mock<IValidator<AdjustInventoryExitCommand>> _validatorMock;
    private readonly Mock<IEventBus> _eventBusMock = new();
    private readonly AdjustInventoryExitCommandHandler _handler;

    public AdjustInventoryExitCommandHandlerTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _inventoryRepositoryMock = _fixture.Freeze<Mock<IInventoryRepository>>();
        _inventoryHistoryRepositoryMock = _fixture.Freeze<Mock<IInventoryHistoryRepository>>();
        _validatorMock = _fixture.Freeze<Mock<IValidator<AdjustInventoryExitCommand>>>();
        _handler = new AdjustInventoryExitCommandHandler(_inventoryRepositoryMock.Object,
                    _inventoryHistoryRepositoryMock.Object,
                    _validatorMock.Object,
                    _eventBusMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldAdjustInventory_WhenRequestIsValid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var action = ActionType.Exit;
        var quantity = 10;
        var inventory = _fixture
                        .Build<Inventory>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Quantity, 100)
                        .Create();
        var expectedInventory = _fixture
                        .Build<Inventory>()
                        .With(p => p.ProductId, productId)
                        .With(p => p.Quantity, inventory.Quantity - quantity)
                        .Create();
        var command = _fixture.Build<AdjustInventoryExitCommand>()
                              .With(c => c.ProductId, productId)
                              .With(c => c.Quantity, quantity)
                              .Create();
        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _inventoryRepositoryMock
            .Setup(repo => repo.AdjustInventoryAsync(productId, (Domain.Enums.ActionType)action, quantity))
            .ReturnsAsync((expectedInventory, inventory.Quantity));
        var inventoryHistory = new InventoryHistory
        {
            InventoryId = expectedInventory.Id,
            ProductId = expectedInventory.ProductId,
            OldQuantity = inventory.Quantity,
            NewQuantity = expectedInventory.Quantity,
            Timestamp = DateTime.UtcNow
        };
        _inventoryHistoryRepositoryMock
            .Setup(repo => repo.CreateInventoryHistoryAsync(inventoryHistory))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var resultValue = result
            .Match(
            productId => productId,
            _ => default!,
            _ => default!);
        resultValue.Should().Be(productId);
        inventoryHistory.NewQuantity.Should().Be(90);

        _inventoryRepositoryMock.Verify(repo => repo.AdjustInventoryAsync(productId, (Domain.Enums.ActionType)action, quantity), Times.Once);
        _inventoryHistoryRepositoryMock.Verify(repo => repo.CreateInventoryHistoryAsync(It.IsAny<InventoryHistory>()), Times.Once);
        _eventBusMock.Verify(eventBus => eventBus.PublishAsync(It.IsAny<InventoryAdjustedEvent>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnValidationErrors_WhenRequestIsInvalid()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var quantity = 10;
        var command = _fixture.Build<AdjustInventoryExitCommand>()
                              .With(c => c.ProductId, productId)
                              .With(c => c.Quantity, quantity)
                              .Create();
        var validationResult = new ValidationResult(new List<ValidationFailure>
        {
            new("ProductId", "ProductId is required."),
            new("Action", "Action is required."),
            new("Quantity", "Quantity is required.")
        });
        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsError.Should().BeTrue();
        var resultError = result.Match(
            _ => default!,
            validationFailed => validationFailed,
            _ => default!);
        resultError.Should().NotBeNull();
        resultError.Errors.Should().HaveCount(3);
        resultError.Errors.Should().ContainSingle(e => e.ErrorMessage == "ProductId is required.");
        resultError.Errors.Should().ContainSingle(e => e.ErrorMessage == "Action is required.");
        resultError.Errors.Should().ContainSingle(e => e.ErrorMessage == "Quantity is required.");

        _inventoryRepositoryMock.Verify(repo => repo.AdjustInventoryAsync(It.IsAny<Guid>(), It.IsAny<Domain.Enums.ActionType>(), It.IsAny<int>()), Times.Never);
        _inventoryHistoryRepositoryMock.Verify(repo => repo.CreateInventoryHistoryAsync(It.IsAny<InventoryHistory>()), Times.Never);
        _eventBusMock.Verify(eventBus => eventBus.PublishAsync(It.IsAny<InventoryAdjustedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldReturnRecordNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var action = ActionType.Exit;
        var quantity = 10;
        var command = _fixture.Build<AdjustInventoryExitCommand>()
                              .With(c => c.ProductId, productId)
                              .With(c => c.Quantity, quantity)
                              .Create();
        var validationResult = new ValidationResult();
        _validatorMock
            .Setup(validator => validator.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);
        _inventoryRepositoryMock
            .Setup(repo => repo.AdjustInventoryAsync(productId, (Domain.Enums.ActionType)action, quantity))
            .ReturnsAsync((default(Inventory)!, default));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        var resultError = result.Match(
            _ => default!,
            _ => default!,
            recordNotFound => recordNotFound);
        resultError.Should().NotBeNull();
        resultError.Messages.Should().ContainSingle(e => e == $"Inventory with Product Id {productId} not found.");

        _inventoryRepositoryMock.Verify(repo => repo.AdjustInventoryAsync(productId, (Domain.Enums.ActionType)action, quantity), Times.Once);
        _inventoryHistoryRepositoryMock.Verify(repo => repo.CreateInventoryHistoryAsync(It.IsAny<InventoryHistory>()), Times.Never);
        _eventBusMock.Verify(eventBus => eventBus.PublishAsync(It.IsAny<InventoryAdjustedEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
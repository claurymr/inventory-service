using FluentValidation;
using InventoryService.Application.Inventories.AdjustInventories;

namespace InventoryService.Application.Validation.Validators;
/// <summary>
/// Validator for the <see cref="AdjustInventoryExitCommand"/> class.
/// </summary>
public class AdjustInventoryExitCommandValidator : AbstractValidator<AdjustInventoryExitCommand>
{
    public AdjustInventoryExitCommandValidator()
    {
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required");

        RuleFor(x => x.Quantity)
            .NotEmpty().WithMessage("Quantity is required")
            .GreaterThan(0).WithMessage("Quantity must be greater than 0");
    }
}
using FluentValidation.Results;

namespace InventoryService.Application.Validation;
public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(ValidationFailure error) : this([error])
    {
    }
}
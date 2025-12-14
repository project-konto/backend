using FluentValidation;

namespace KontoApi.Application.Features.Budgets.Commands.RenameBudget;

public class RenameBudgetValidator : AbstractValidator<RenameBudgetCommand>
{
    public RenameBudgetValidator()
    {
        RuleFor(x => x.BudgetId)
            .NotEmpty();

        RuleFor(x => x.NewName)
            .NotEmpty().WithMessage("New budget name is required")
            .MaximumLength(100).WithMessage("Budget name must not exceed 100 characters");
    }
}

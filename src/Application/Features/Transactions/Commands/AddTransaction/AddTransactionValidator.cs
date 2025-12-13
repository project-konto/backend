using FluentValidation;

namespace KontoApi.Application.Features.Transactions.Commands.AddTransaction;

public class AddTransactionValidator : AbstractValidator<AddTransactionCommand>
{
    public AddTransactionValidator()
    {
        RuleFor(x => x.BudgetId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Currency).Length(3);
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
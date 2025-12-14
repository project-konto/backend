using FluentValidation;

namespace KontoApi.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionCommand>
{
    public DeleteTransactionValidator()
    {
        RuleFor(x => x.BudgetId).NotEmpty();
        RuleFor(x => x.TransactionId).NotEmpty();
    }
}

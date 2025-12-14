using FluentValidation;

namespace KontoApi.Application.Features.Transactions.Commands.ImportTransactions;

public class ImportTransactionsValidator : AbstractValidator<ImportTransactionsCommand>
{
    public ImportTransactionsValidator()
    {
        RuleFor(x => x.BudgetId).NotEmpty();
        RuleFor(x => x.FileStream).NotNull();

        RuleFor(x => x.FileName)
            .NotEmpty()
            .Must(f => f.EndsWith(".csv", StringComparison.OrdinalIgnoreCase) ||
                       f.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Only .csv and .pdf files are supported");
    }
}

using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.ImportTransactions;

public class ImportTransactionsHandler : IRequestHandler<ImportTransactionsCommand, ImportResultDto>
{
    private readonly IBudgetRepository budgetRepository;
    private readonly IStatementParser statementParser;
    private readonly IApplicationDbContext dbContext;

    public ImportTransactionsHandler(
        IBudgetRepository budgetRepository,
        IStatementParser statementParser,
        IApplicationDbContext dbContext)
    {
        this.budgetRepository = budgetRepository;
        this.statementParser = statementParser;
        this.dbContext = dbContext;
    }

    public async Task<ImportResultDto> Handle(ImportTransactionsCommand request, CancellationToken cancellationToken)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(typeof(Budget), request.BudgetId);

        var parsedRecords = statementParser.Parse(request.FileStream);

        var success = 0;
        var failed = 0;
        var errors = new List<string>();

        foreach (var record in parsedRecords)
        {
            try
            {
                var category = await dbContext.Categories.FindAsync([record.CategoryId], cancellationToken);
                if (category == null)
                {
                    throw new($"Category {record.CategoryId} not found");
                }

                var money = new Money(record.Amount, record.Currency);
                var tx = new Transaction(money, record.Type, category, record.Date, record.Description);

                budget.AddTransaction(tx);
                success++;
            }
            catch (Exception ex)
            {
                failed++;
                errors.Add($"Row error: {ex.Message}");
            }
        }

        await budgetRepository.UpdateAsync(budget, cancellationToken);

        return new(parsedRecords.Count, success, failed, errors);
    }
}
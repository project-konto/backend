using KontoApi.Application.Exceptions;
using KontoApi.Application.Users.Transactions;
using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Handlers;

public class AddTransactionHandler
{
    private readonly IBudgetRepository budgetRepository;

    public AddTransactionHandler(IBudgetRepository repository) => budgetRepository = repository;

    public async Task<AddTransactionDto> Handle(AddTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.BudgetId == Guid.Empty)
            throw new BadRequestException("Budget id cannot be empty");
        if (command.Amount <= 0)
            throw new ValidationException("Amount must be greater than 0");
        if (string.IsNullOrWhiteSpace(command.Currency))
            throw new ValidationException("Currency is required");
        if (command.Date > DateTime.UtcNow)
            throw new ValidationException("Transaction date cannot be in the future");

        var money = new Money(command.Amount, command.Currency);
        var category = new Category(command.Category);
        var transaction =
            new Transaction(money, command.Type, category, command.Date, command.Description ?? string.Empty);

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);

        if (budget == null)
            throw new NotFoundException(typeof(Budget), command.BudgetId);

        budget.AddTransaction(transaction);

        await budgetRepository.UpdateAsync(budget, cancellationToken);

        return new AddTransactionDto
        {
            TransactionId = transaction.Id,
            BudgetId = command.BudgetId,
        };
    }
}

public class DeleteTransactionHandler
{
    private readonly IBudgetRepository budgetRepository;

    public DeleteTransactionHandler(IBudgetRepository repository) => budgetRepository = repository;

    public async Task<DeleteTransactionDto> Handle(DeleteTransactionCommand command,
        CancellationToken cancellationToken = default)
    {
        if (command.BudgetId == Guid.Empty)
            throw new BadRequestException("Budget id cannot be empty");
        if (command.TransactionId == Guid.Empty)
            throw new BadRequestException("Transaction id cannot be empty");

        var budget = await budgetRepository.GetByIdAsync(command.BudgetId, cancellationToken);
        if (budget == null)
            throw new NotFoundException(typeof(Budget), command.BudgetId);

        var transaction = await budgetRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction == null)
            throw new NotFoundException(typeof(Transaction), command.TransactionId);

        budget.RemoveTransaction(command.TransactionId);
        await budgetRepository.UpdateAsync(budget, cancellationToken);

        return new DeleteTransactionDto
        {
            IsDeleted = true
        };
    }
}

public class ImportTransactionsHandler
{
    private readonly IStatementParser statementParser;
    private readonly IBudgetRepository budgetRepository;

    public ImportTransactionsHandler(IStatementParser parser, IBudgetRepository repository)
    {
        statementParser = parser;
        budgetRepository = repository;
    }

    public async Task<ImportTransactionsDto> Handle(ImportTransactionCommand command,
        CancellationToken cancellationToken)
    {
        if (command.BudgetId == Guid.Empty)
            throw new BadRequestException("Budget id cannot be empty");

        var stream = new MemoryStream(command.FileBytes);
        var parsed = statementParser.Parse(stream).ToList();
        var skipped = 0;
        var lineNumber = 0;
        var errors = new List<ImportErrorDto>();
        var transactions = new List<Transaction>();

        foreach (var operation in parsed)
        {
            lineNumber++;
            try
            {
                var category = operation.CategoryName ?? new Category("Uncategorized"); // temporary 
                var money = new Money(Math.Abs(operation.Amount), operation.Currency);
                var type = operation.Amount > 0 ? TransactionType.Income : TransactionType.Expense;
                var transaction = new Transaction(money, type, category, operation.Date,
                    operation.Description ?? string.Empty);

                transactions.Add(transaction);
            }

            catch (Exception ex)
            {
                errors.Add(new ImportErrorDto
                {
                    LineNumber = lineNumber,
                    Reason = ex.Message
                });
            }
        }

        // Logic to update budget with new transactions

        return new ImportTransactionsDto
        {
            Total = parsed.Count,
            Imported = transactions.Count,
            SkippedDuplicates = skipped,
            Errors = errors
        };
    }
}
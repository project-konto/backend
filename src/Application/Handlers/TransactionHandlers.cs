using KontoApi.Application.Exceptions;
using KontoApi.Application.Users.Transactions;
using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Handlers;

public class AddTransactionHandler
{
    private readonly ITransactionRepository transactionRepository;

    public AddTransactionHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<AddTransactionDto> Handle(AddTransactionCommand command)
    {
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

        await transactionRepository.AddAsync(transaction);
        return new()
        {
            TransactionId = transaction.Id
        };
    }
}

public class DeleteTransactionHandler
{
    private readonly ITransactionRepository transactionRepository;

    public DeleteTransactionHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<DeleteTransactionDto> Handle(DeleteTransactionCommand command)
    {
        var exists = await transactionRepository.ExistsAsync(command.TransactionId);
        if (!exists)
            throw new NotFoundException($"Transaction with id {command.TransactionId} does not exist");

        await transactionRepository.DeleteAsync(command.TransactionId);
        return new()
        {
            IsDeleted = true
        };
    }
}

public class ImportTransactionsHandler
{
    private readonly IStatementParser statementParser;
    private readonly ITransactionRepository transactionRepository;

    public ImportTransactionsHandler(IStatementParser parser, ITransactionRepository repository)
    {
        statementParser = parser;
        transactionRepository = repository;
    }

    public async Task<ImportTransactionsDto> Handle(ImportTransactionCommand command)
    {
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
                if (operation.ExternalId != null &&
                    await transactionRepository.ExistsByExternalIdAsync(command.UserId, operation.ExternalId))
                {
                    skipped++;
                    continue;
                }

                var category = operation.CategoryName ?? new Category("Uncategorized"); // temporary 
                var money = new Money(Math.Abs(operation.Amount), operation.Currency);
                var type = operation.Amount > 0 ? TransactionType.Income : TransactionType.Expense;
                var transaction = new Transaction(money, type, category, operation.Date,
                    operation.Description ?? string.Empty);

                transactions.Add(transaction);
            }

            catch (Exception ex)
            {
                errors.Add(new()
                {
                    LineNumber = lineNumber,
                    Reason = ex.Message
                });
            }
        }

        await transactionRepository.AddRangeAsync(transactions);
        return new()
        {
            Total = parsed.Count,
            Imported = transactions.Count,
            SkippedDuplicates = skipped,
            Errors = errors
        };
    }
}
using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users.Transactions;

public class ImportTransactionCommand
{
    public Guid UserId { get; init; }
    public byte[] FileBytes { get; init; } = [];
    public string FileName { get; init; } = string.Empty;
    public string? ContentType { get; init; }
}

public class ImportTransactionsDto
{
    public int Total { get; init; }
    public int Imported { get; init; }
    public int SkippedDuplicates { get; init; }
    public List<ImportErrorDto> Errors { get; init; } = [];
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

                var category = operation.Category ?? new Category("Uncategorized"); // temporary 
                var money = new Money(Math.Abs((double)operation.Amount), operation.Currency);
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
                    Reason = ex.Message,
                });
            }
        }

        await transactionRepository.AddRangeAsync(transactions);
        return new ImportTransactionsDto
        {
            Total = parsed.Count,
            Imported = transactions.Count,
            SkippedDuplicates = skipped, 
            Errors = errors
        };
    }
}
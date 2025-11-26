using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users.Transactions;

public class AddTransactionCommand
{
    public Guid UserId { get; init; }
    public TransactionType Type { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "";
    public string Category { get; init; } = "";
    public DateTime Date { get; init; }
    public string? Description { get; init; }
}

public class AddTransactionDto
{
    public Guid TransactionId { get; init; }
}

public class AddTransactionHandler
{
    private readonly ITransactionRepository transactionRepository;

    public AddTransactionHandler(ITransactionRepository repository) => transactionRepository = repository;

    public async Task<AddTransactionDto> Handle(AddTransactionCommand command)
    {
        if (command.Amount <= 0)
            throw new ArgumentException("Amount must be greater than 0");
        if (string.IsNullOrWhiteSpace(command.Currency))
            throw new ArgumentException("Currency is required");
        if (command.Date > DateTime.UtcNow)
            throw new ArgumentException("Transaction date cannot be in the future");

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
namespace KontoApi.Domain;

public class Budget
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public Money CurrentBalance { get; private set; }

    private readonly List<Transaction> transactions = [];

    public IReadOnlyCollection<Transaction> Transactions
        => transactions.AsReadOnly();

    public Budget(string name, Money initialBalance)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Budget name cannot be empty", nameof(name));

        Id = Guid.NewGuid();
        Name = name.Trim();
        CurrentBalance = initialBalance ?? throw new ArgumentNullException(nameof(initialBalance));
    }

    private Budget()
    {
        ; // For ORM
    }


    public void AddTransaction(Transaction transaction)
    {
        ArgumentNullException.ThrowIfNull(transaction);

        if (transaction.Amount.Currency != CurrentBalance.Currency)
            throw new ArgumentException(
                $"Transaction currency ({transaction.Amount.Currency}) must match budget currency ({CurrentBalance.Currency})");

        transactions.Add(transaction);
        UpdateBalanceFromTransaction(transaction);
    }

    public void RemoveTransaction(Guid transactionId)
    {
        var transaction = transactions.FirstOrDefault(t => t.Id == transactionId);
        if (transaction is null)
            throw new InvalidOperationException($"Transaction {transactionId} not found in budget");

        transactions.Remove(transaction);
        ReverseBalanceFromTransaction(transaction);
    }

    public Money GetTotalIncome()
    {
        var total = transactions
            .Where(t => t.Type == TransactionType.Income)
            .Sum(t => t.Amount.Value);

        return new(total, CurrentBalance.Currency);
    }

    public Money GetTotalExpenses()
    {
        var total = transactions
            .Where(t => t.Type == TransactionType.Expense)
            .Sum(t => t.Amount.Value);

        return new(total, CurrentBalance.Currency);
    }

    public IEnumerable<Transaction> GetTransactionsByDateRange(DateRange dateRange)
        => dateRange is null
            ? throw new ArgumentNullException(nameof(dateRange))
            : transactions.Where(t => dateRange.Contains(t.Date));

    public IEnumerable<Transaction> GetTransactionsByCategory(Category category)
        => category is null
            ? throw new ArgumentNullException(nameof(category))
            : transactions.Where(t => t.TransactionCategory.Equals(category));

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Budget name cannot be empty", nameof(newName));

        Name = newName.Trim();
    }

    private void UpdateBalanceFromTransaction(Transaction transaction)
    {
        CurrentBalance = transaction.Type switch
        {
            TransactionType.Income => CurrentBalance + transaction.Amount,
            TransactionType.Expense => CurrentBalance - transaction.Amount,
            TransactionType.Transfer => CurrentBalance,
            _ => throw new InvalidOperationException($"Unknown transaction type: {transaction.Type}")
        };
    }

    private void ReverseBalanceFromTransaction(Transaction transaction)
    {
        CurrentBalance = transaction.Type switch
        {
            TransactionType.Income => CurrentBalance - transaction.Amount,
            TransactionType.Expense => CurrentBalance + transaction.Amount,
            TransactionType.Transfer => CurrentBalance,
            _ => throw new InvalidOperationException($"Unknown transaction type: {transaction.Type}")
        };
    }

    public override string ToString()
        => $"{Name}: {CurrentBalance}";
}
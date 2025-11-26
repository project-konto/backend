namespace KontoApi.Domain;

public enum TransactionType
{
    Income,
    Expense,
    Transfer
}

public class Transaction
{
    public Guid Id { get; private set; }
    public TransactionType Type { get; private set; }
    public Money Amount { get; private set; }
    public Category TransactionCategory { get; private set; }
    public DateTime Date { get; private set; }
    public string Description { get; private set; }

    public Transaction(Money amount, TransactionType type, Category category, DateTime date, string? description = null)
    {
        ArgumentNullException.ThrowIfNull(amount);
        ArgumentNullException.ThrowIfNull(category);

        if (amount.Value <= 0)
            throw new ArgumentException("Transaction amount must be greater than zero", nameof(amount));

        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        TransactionCategory = category;
        Date = date;
        Description = description?.Trim() ?? string.Empty;
    }

    public void UpdateCategory(Category newCategory)
    {
        ArgumentNullException.ThrowIfNull(newCategory);
        TransactionCategory = newCategory;
    }

    public void UpdateDescription(string newDescription)
        => Description = newDescription.Trim();

    public override string ToString()
        => $"{Type}: {Amount} - {TransactionCategory.Name} on {Date:yyyy-MM-dd}";
}
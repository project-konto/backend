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

    public Transaction(Money amount, TransactionType type, Category category, DateTime date, string description)
    {
        Amount = amount;
        Type = type;
        TransactionCategory = category;
        Date = date;
        Description = description;
        Id = Guid.NewGuid();
    }
}
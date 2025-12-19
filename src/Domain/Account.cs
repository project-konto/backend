namespace KontoApi.Domain;

public class Account
{
    public Guid Id { get; private set; }
    public User User { get; init; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private readonly List<Budget> budgets = [];
    public IReadOnlyCollection<Budget> Budgets => budgets.AsReadOnly();

    public Account(User user, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Account name cannot be empty", nameof(name));

        Id = Guid.NewGuid();
        User = user ?? throw new ArgumentNullException(nameof(user));
        Name = name.Trim();
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
        budgets.Add(new Budget("Default", new Money(0, "RUB")));
    }

    private Account()
    {
        /* For ORM */
    }


    public void AddBudget(Budget budget)
    {
        ArgumentNullException.ThrowIfNull(budget);

        if (budgets.Any(b => b.Id == budget.Id))
            throw new InvalidOperationException($"Budget {budget.Id} already exists in account");

        budgets.Add(budget);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Account name cannot be empty", nameof(newName));

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveBudget(Guid budgetId)
    {
        var budget = budgets.FirstOrDefault(b => b.Id == budgetId);
        if (budget is null)
            throw new InvalidOperationException($"Budget {budgetId} not found in account");

        budgets.Remove(budget);
        UpdatedAt = DateTime.UtcNow;
    }

    public Budget? GetBudgetById(Guid budgetId)
        => budgets.FirstOrDefault(b => b.Id == budgetId);

    public IEnumerable<Budget> GetBudgetsByCurrency(string currency)
        => string.IsNullOrWhiteSpace(currency)
            ? throw new ArgumentException("Currency cannot be empty", nameof(currency))
            : budgets.Where(b => b.CurrentBalance.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase));

    public Money GetTotalBalanceByCurrency(string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be empty", nameof(currency));

        var total = budgets
            .Where(b => b.CurrentBalance.Currency.Equals(currency, StringComparison.OrdinalIgnoreCase))
            .Sum(b => b.CurrentBalance.Value);

        return new(total, currency.ToUpperInvariant());
    }

    public override string ToString()
        => $"Account for {User.Name} with {budgets.Count} budget(s)";
}
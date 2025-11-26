namespace KontoApi.Domain;

public class Account(User user)
{
	public Guid Id { get; private set; } = Guid.NewGuid();
	public User User { get; private set; } = user ?? throw new ArgumentNullException(nameof(user));
	public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; private set; } = DateTime.UtcNow;

	private readonly List<Budget> budgets = [];
	public IReadOnlyCollection<Budget> Budgets => budgets.AsReadOnly();

	public void AddBudget(Budget budget)
	{
		ArgumentNullException.ThrowIfNull(budget);

		if (budgets.Any(b => b.Id == budget.Id))
			throw new InvalidOperationException($"Budget {budget.Id} already exists in account");

		budgets.Add(budget);
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
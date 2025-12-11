namespace KontoApi.Application.DTOs;

public class BudgetDto
{
    public Guid Id { get; init; }
    public decimal CurrentBalance { get; init; }
    public string Currency { get; init; } = "";
    public int TransactionsCount { get; init; }
}
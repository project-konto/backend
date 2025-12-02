namespace KontoApi.Infrastructure.Models;

public class BudgetEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public decimal CurrentBalance { get; set; }
    public Guid AccountId { get; set; }
    public string Currency { get; set; }
    public AccountEntity? AccountEntity { get; set; }
}
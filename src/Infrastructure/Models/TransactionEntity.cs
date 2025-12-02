namespace KontoApi.Infrastructure.Models;

public class TransactionEntity
{
    public Guid Id { get; set; }
    public string TransactionTypeName { get; set; }
    public decimal Amount { get; set; }
    public Guid CategoryId { get; set; }
    public DateTime TimeTransaction { get; set; }
    public string? Description { get; set; }
    public Guid BudgetId { get; set; }
    public BudgetEntity? BudgetEntity { get; set; }
    public CategoryEntity? CategoryEntity { get; set; }
    public TransactionTypesEntity? TransactionTypesEntity { get; set; }

    public void SetTypeTransaction(string typeTransactionName)
    {
        TransactionTypeName = typeTransactionName;
    }
}
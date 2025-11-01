namespace KontoApi.Domain;

public class Budget
{
    public Guid Id { get; private set; }

    public Money CurrentBalance { get; private set; }
    public Transaction[] Transactions { get; set; }

    public Budget(Money currentBalance, Transaction[] transactions)
    {
        throw new NotImplementedException();
    }
}
namespace KontoApi.Infrastructure.Models;

public class TransactionTypesEntity
{
    public string Name { get; set; }

    public TransactionTypesEntity(string name)
    {
        Name = name;
    }

    public TransactionTypesEntity()
    {
    }
}
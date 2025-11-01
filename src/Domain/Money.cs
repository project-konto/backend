namespace KontoApi.Domain;


public class Money: IEquatable<Money>, IComparable<Money>
{
    public double Value {get; private set;}
    public string Currency {get; private set;}
    
    public Money(double value, string currency) {}

    public Money Add(Money other)
    {
        throw new NotImplementedException();
    }

    public Money Subtract(Money other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Money? other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(Money? other)
    {
        throw new NotImplementedException();
    }
}
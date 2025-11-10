namespace KontoApi.Domain;

public class Money : IEquatable<Money>, IComparable<Money>
{
    public double Value { get; private set; }
    public string Currency { get; private set; }

    public Money(double value, string currency)
    {
    }

    public bool Equals(Money? other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(Money? other)
    {
        throw new NotImplementedException();
    }

    public static Money operator +(Money x, Money y) => new Money(x.Value + x.Value, x.Currency);
    public static Money operator -(Money x, Money y) => new Money(x.Value - x.Value, x.Currency);
    public static Money operator *(Money x, Money y) => new Money(x.Value * y.Value, x.Currency);
}
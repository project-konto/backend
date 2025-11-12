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

    public static Money operator +(Money firstItem, Money secondItem) => new Money(firstItem.Value + secondItem.Value, firstItem.Currency);
    public static Money operator -(Money firstItem, Money secondItem) => new Money(firstItem.Value - secondItem.Value, firstItem.Currency);
}
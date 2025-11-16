namespace KontoApi.Domain;

public class Money : IEquatable<Money>, IComparable<Money>
{
    public double Value { get; private set; }
    public string Currency { get; private set; }
    const double Epsilon = 0.0000001;


    public Money(double value, string currency)
    {
        Value = value;
        Currency = currency;
    }

    public bool Equals(Money other) => Math.Abs(Value - other.Value) < Epsilon && Currency == other.Currency;

    public int CompareTo(Money? other)
    {
        throw new NotImplementedException();
    }

    public static Money operator +(Money firstItem, Money secondItem) => new Money(firstItem.Value + secondItem.Value, firstItem.Currency);
    public static Money operator -(Money firstItem, Money secondItem) => new Money(firstItem.Value - secondItem.Value, firstItem.Currency);
}
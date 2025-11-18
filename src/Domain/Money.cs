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

    public int CompareTo(Money other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return this.Currency == other.Currency
            ? Value.CompareTo(other.Value)
            : throw new ArgumentException($"Cannot compare {nameof(Money)} with {nameof(Currency)}.");
    }

    public static Money operator +(Money firstItem, Money secondItem) =>
        new Money(firstItem.Value + secondItem.Value, firstItem.Currency);

    public static Money operator -(Money firstItem, Money secondItem) =>
        new Money(firstItem.Value - secondItem.Value, firstItem.Currency);
}
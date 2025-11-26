namespace KontoApi.Domain;

public class Money : IEquatable<Money>, IComparable<Money>
{
	public decimal Value { get; }
	public string Currency { get; }


	public Money(decimal value, string currency)
	{
		if (string.IsNullOrWhiteSpace(currency))
			throw new ArgumentNullException(nameof(currency), "Currency cannot be empty");
		if (currency.Length != 3)
			throw new ArgumentOutOfRangeException(nameof(currency), "Currency must be a 3-character ISO code");

		Value = value;
		Currency = currency.Trim().ToUpperInvariant();
	}

	public bool Equals(Money? other)
		=> other != null
		   && Value == other.Value
		   && Currency == other.Currency;

	public override bool Equals(object? obj)
		=> obj is Money other && Equals(other);

	public override int GetHashCode()
		=> HashCode.Combine(Value, Currency);

	public int CompareTo(Money? other)
	{
		if (ReferenceEquals(this, other))
			return 0;

		if (ReferenceEquals(null, other))
			return 1;

		return Currency == other.Currency
			? Value.CompareTo(other.Value)
			: throw new ArgumentException($"Cannot compare {other.Currency} with {Currency}");
	}

	public static Money operator +(Money first, Money second) =>
		first.Currency != second.Currency
			? throw new ArgumentException($"Cannot add {first.Currency} and {second.Currency}")
			: new(first.Value + second.Value, first.Currency);


	public static Money operator -(Money first, Money second) =>
		first.Currency != second.Currency
			? throw new ArgumentException($"Cannot subtract {second.Currency} from {first.Currency}")
			: new(first.Value - second.Value, first.Currency);

	public static bool operator ==(Money? left, Money? right)
		=> Equals(left, right);

	public static bool operator !=(Money? left, Money? right)
		=> !Equals(left, right);

	public static bool operator >(Money left, Money right)
		=> left.CompareTo(right) > 0;

	public static bool operator <(Money left, Money right)
		=> left.CompareTo(right) < 0;

	public static bool operator >=(Money left, Money right)
		=> left.CompareTo(right) >= 0;

	public static bool operator <=(Money left, Money right)
		=> left.CompareTo(right) <= 0;

	public override string ToString()
		=> $"{Value:F2} {Currency}";
}
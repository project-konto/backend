namespace KontoApi.Domain;

public class DateRange : IEquatable<DateRange>, IComparable<DateRange>
{
	public DateTime StartDate { get; }
	public DateTime EndDate { get; }
	public TimeSpan Length => EndDate - StartDate;

	public DateRange(DateTime startDate, DateTime endDate)
	{
		if (startDate > endDate)
			throw new ArgumentException("Start date must be before or equal to end date");

		StartDate = startDate;
		EndDate = endDate;
	}

	public bool Contains(DateTime date)
		=> date >= StartDate && date <= EndDate;


	public bool Equals(DateRange? other)
	{
		if (other is null) return false;
		if (ReferenceEquals(this, other)) return true;
		return StartDate == other.StartDate && EndDate == other.EndDate;
	}

	public override bool Equals(object? obj)
		=> obj is DateRange other && Equals(other);

	public override int GetHashCode()
		=> HashCode.Combine(StartDate, EndDate);

	public int CompareTo(DateRange? other)
		=> other is null
			? 1
			: Length.CompareTo(other.Length);

	public override string ToString()
		=> $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
}
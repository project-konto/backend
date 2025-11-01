using System.Runtime.InteropServices.JavaScript;

namespace KontoApi.Domain;

public class DateRange: IEquatable<DateRange>, IComparable<DateRange>
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    
    public DateRange(DateTime startDate, DateTime endDate)
    {
        throw new NotImplementedException();
    }

    public bool Equals(DateRange? other)
    {
        throw new NotImplementedException();
    }

    public int CompareTo(DateRange? other)
    {
        throw new NotImplementedException();
    }
}
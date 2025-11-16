using System.Runtime.InteropServices.JavaScript;

namespace KontoApi.Domain;

public class DateRange: IEquatable<DateRange>, IComparable<DateRange>
{
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    
    public DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate;
        EndDate = endDate;
    }

    public bool Equals(DateRange other) => EndDate == other.EndDate && StartDate == other.StartDate;
    

    public int CompareTo(DateRange? other)
    {
        throw new NotImplementedException();
    }
}
namespace KontoApi.Domain;

public class Category: IComparable<Category>, IEquatable<Category>
{
    public string Name { get; private set; } 

    public Category(string name) 
    {      
        throw new NotImplementedException();
    }

    public int CompareTo(Category? other)
    {
        throw new NotImplementedException();
    }

    public bool Equals(Category? other)
    {
        throw new NotImplementedException();
    }
}
namespace KontoApi.Domain;

public class Category: IComparable<Category>, IEquatable<Category>
{
    public string Name { get; private set; }

    public Category(string name)
    {
        Name = name;
    }

    public bool Equals(Category other) => other.Name == Name;

    public int CompareTo(Category other) => String.Compare(Name, other.Name, StringComparison.Ordinal);
}
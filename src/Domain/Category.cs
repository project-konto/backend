namespace KontoApi.Domain;

public class Category : IComparable<Category>, IEquatable<Category>
{
    public Guid Id { get; }
    public string Name { get; private set; }

    public Category(string name)
    {
        ValidateName(name, nameof(name));
        Id = Guid.NewGuid();
        Name = name.Trim();
    }

    private Category()
    {
        ; // For ORM
    }

    public void Rename(string newName)
    {
        ValidateName(newName, nameof(newName));
        Name = newName.Trim();
    }

    public bool Equals(Category? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;

        return Id == other.Id;
    }

    public override bool Equals(object? obj)
        => obj is Category other && Equals(other);

    public override int GetHashCode()
        => Id.GetHashCode();

    public int CompareTo(Category? other)
    {
        return other is null
            ? 1
            : string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override string ToString()
        => Name;

    public static bool operator ==(Category? left, Category? right)
        => Equals(left, right);

    public static bool operator !=(Category? left, Category? right)
        => !Equals(left, right);


    private static void ValidateName(string name, string paramName)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Category name cannot be empty", paramName);

        if (name.Length > 80)
            throw new ArgumentException("Category name cannot exceed 80 characters", paramName);
    }
}
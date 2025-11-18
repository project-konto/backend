namespace KontoApi.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }

    private readonly List<Budget> budgets = new List<Budget>();

    
    public User(string name, string email, string hashedPassword)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        HashedPassword = hashedPassword;
    }

    public void ChangeName(string newName)
    {
        Name = newName;
    }
}
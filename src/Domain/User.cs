namespace KontoApi.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    private string HashedPassword { get; set; }

    private readonly List<Budget> _budgets = new List<Budget>();

    
    public User(Guid id, string name, string email, string hashedPassword)
    {
        throw new NotImplementedException();

    }

    public static User Register(string name, string email, string password)
    {
        throw new NotImplementedException();
    }

    public void ChangeName(string newName)
    {
        throw new NotImplementedException();
    }

}
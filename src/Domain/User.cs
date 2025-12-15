namespace KontoApi.Domain;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public string HashedPassword { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public User(string name, string email, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (!IsValidEmail(email))
            throw new ArgumentException("Invalid email format", nameof(email));

        if (string.IsNullOrWhiteSpace(hashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(hashedPassword));

        Id = Guid.NewGuid();
        Name = name.Trim();
        Email = email.Trim().ToLowerInvariant();
        HashedPassword = hashedPassword;
        CreatedAt = DateTime.UtcNow;
    }

    private User()
    {
        /* For ORM */
    }

    public void ChangeName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Name cannot be empty", nameof(newName));

        Name = newName.Trim();
    }

    public void ChangeEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("Email cannot be empty", nameof(newEmail));

        if (!IsValidEmail(newEmail))
            throw new ArgumentException("Invalid email format", nameof(newEmail));

        Email = newEmail.Trim().ToLowerInvariant();
    }

    public void ChangePassword(string newHashedPassword)
    {
        if (string.IsNullOrWhiteSpace(newHashedPassword))
            throw new ArgumentException("Hashed password cannot be empty", nameof(newHashedPassword));

        HashedPassword = newHashedPassword;
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email.Trim();
        }
        catch
        {
            return false;
        }
    }

    public override string ToString() => $"{Name} ({Email})";
}
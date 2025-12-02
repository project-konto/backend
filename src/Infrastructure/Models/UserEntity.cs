namespace KontoApi.Infrastructure.Models;

public class UserEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string HashedPassword { get; set; }
    public DateTime CreatedAt { get; set; }
}
namespace KontoApi.Infrastructure.Models;

public class AccountEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public UserEntity? UserEntity { get; set; }
}
namespace KontoApi.Application.Users;

public class LogoutUserCommand
{
    public Guid UserId { get; init; }

    public LogoutUserCommand(Guid userId) => UserId = userId;
}
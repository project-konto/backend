namespace KontoApi.Application.Users;

public class LogoutUserCommand
{
    public Guid UserId { get; init; }

    public LogoutUserCommand(Guid userId)
    {
        UserId = userId;
    }

    public class LogoutUserHandler
    {
        public Task Handle(LogoutUserCommand command) => Task.CompletedTask;
    }
}
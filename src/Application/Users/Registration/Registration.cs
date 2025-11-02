using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Users;

public class RegisterUserCommand
{
    // Gets data (email, password) from UI and sends to handler
    public string Email { get; }
    public string Password { get; }

    public RegisterUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}

public class RegisterUserHandler
{
    // Gets data from RegisterUserCommand and processes them
    private readonly IUserRepository usersRepository;
    private readonly IPasswordHasher passwordHasher;

    public RegisterUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        usersRepository = users;
        passwordHasher = hasher;
    }

    public async Task<RegisterUserResult> Handle(RegisterUserCommand command)
    {
        var email = command.Email;
        var password = command.Password;
        
        var hashed = passwordHasher.Hash(email, password);
        var user = new User(email, hashed);
        
        await usersRepository.AddAsync(user);
        return new RegisterUserResult(user.Id);
    }
}

public class RegisterUserResult
{
    // Outputs the result of processing data
    public bool isSuccess { get; }
    public Guid? UserId { get; }
    public string? ErrorMessage { get; }

    public RegisterUserResult(Guid? userId)
    {
        isSuccess = true;
        UserId = userId;
    }

    public RegisterUserResult(string errorMessage)
    {
        isSuccess = false;
        ErrorMessage = errorMessage;
    }
}
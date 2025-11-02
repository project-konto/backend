using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Users;

public class LoginUserCommand
{
    // Gets data (email, password) from UI and sends to handler
    public string Email { get; }
    public string Password { get; }

    public LoginUserCommand(string email, string password)
    {
        Email = email;
        Password = password;
    }
}

public class LoginUserHandler
{
    private readonly IUserRepository usersRepository;
    private readonly IPasswordHasher passwordHasher;
    
    public LoginUserHandler(IUserRepository users, IPasswordHasher hasher)
    {
        usersRepository = users;
        passwordHasher = hasher;
    }
    
    public async Task<LoginUserResult> Handle(LoginUserCommand command)
    {
        var email = command.Email.Trim();
        var user = await usersRepository.FindByEmailAsync(email);
        
        if (user == null)
            return new LoginUserResult("Invalid email or password");
        
        var isPasswordCorrect = passwordHasher.Verify(email, command.Password, user.PasswordHash);
        if (!isPasswordCorrect)
            return new LoginUserResult("Invalid email or password");

        return new LoginUserResult(user.Id);
    }
}

public class LoginUserResult
{
    public bool IsSuccess { get; }
    public Guid? UserId { get; }
    public string? ErrorMessage { get; }

    public LoginUserResult(Guid? userId)
    {
        IsSuccess = true;
        UserId = userId;
    }
    
    public LoginUserResult(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;;
    }
}
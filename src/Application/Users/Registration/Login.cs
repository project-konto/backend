using KontoApi.Application.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Application.Users;

public class LoginUserCommand
{
    public string Email { get; init; }
    public string Password { get; init; }
}

public class LoginUserResult
{
    public Guid UserId { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
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
       var user = await usersRepository.FindByEmailAsync(command.Email);
       if (user == null)
           throw new InvalidOperationException("Email or password is incorrect");

       var hash = GetHashPassword(user);
       if (!passwordHasher.Verify(command.Password, hash))
           throw new InvalidOperationException("Invalid email or password");

       return new LoginUserResult
       {
           UserId = user.Id,
           Name = user.Name,
           Email = user.Email
       };
    }

    private static string GetHashPassword(User user)
    {
        var property = typeof(User).GetProperty("HashedPassword", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        return (string)property!.GetValue(user);
    }
}
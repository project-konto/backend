using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Queries;

public class GetUserQuery
{
    public Guid UserId { get; init; }
}

public class UserDto
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    public string Email { get; init; }
}

public class GetUserHandler
{
    private readonly IUserRepository userRepository;

    public GetUserHandler(IUserRepository user) => userRepository = user;

    public async Task<UserDto> Handle(GetUserQuery query)
    {
        var user = await userRepository.GetByIdAsync(query.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found");

        return new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
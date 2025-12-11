using KontoApi.Application.Exceptions;
using KontoApi.Application.Queries;
using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;

namespace KontoApi.Application.Handlers;

public class GetUserHandler
{
    private readonly IUserRepository userRepository;

    public GetUserHandler(IUserRepository user) => userRepository = user;

    public async Task<UserDto> Handle(GetUserQuery query)
    {
        var user = await userRepository.GetByIdAsync(query.UserId);
        if (user == null)
            throw new NotFoundException("User not found");

        return new()
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };
    }
}
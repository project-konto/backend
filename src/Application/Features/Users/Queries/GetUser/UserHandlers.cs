using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Users.Queries.GetUser;

public class GetUserHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IUserRepository userRepository;

    public GetUserHandler(IUserRepository userRepository)
        => this.userRepository = userRepository;

    public async Task<UserDto> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
            throw new NotFoundException(typeof(User), request.UserId);

        return new(
            user.Id,
            user.Name,
            user.Email,
            user.CreatedAt
        );
    }
}
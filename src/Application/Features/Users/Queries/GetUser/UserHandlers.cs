using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Users.Queries.GetUser;

public class GetUserHandler(IUserRepository userRepository) : IRequestHandler<GetUserQuery, UserDto>
{
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
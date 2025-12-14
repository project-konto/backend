using KontoApi.Application.DTOs;
using MediatR;

namespace KontoApi.Application.Features.Users.Queries.GetUser;

public record GetUserQuery(Guid UserId) : IRequest<UserDto>;

namespace KontoApi.Application.Features.Users.Queries.GetUser;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTime CreatedAt
);
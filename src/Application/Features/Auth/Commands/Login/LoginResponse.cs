namespace KontoApi.Application.Features.Auth.Commands.Login;

public record LoginResponse(
    string Token,
    Guid UserId,
    string Name,
    string Email
);

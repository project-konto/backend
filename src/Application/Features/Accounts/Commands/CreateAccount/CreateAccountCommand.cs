using MediatR;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public record CreateAccountCommand(Guid UserId) : IRequest<Guid>;
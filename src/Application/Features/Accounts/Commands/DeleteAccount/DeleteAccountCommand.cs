using MediatR;

namespace KontoApi.Application.Features.Accounts.Commands.DeleteAccount;

public record DeleteAccountCommand(Guid AccountId) : IRequest;
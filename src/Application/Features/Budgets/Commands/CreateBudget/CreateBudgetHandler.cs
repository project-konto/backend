using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Budgets.Commands.CreateBudget;

public class CreateBudgetHandler(IAccountRepository accountRepository, ICurrentUserService currentUserService)
    : IRequestHandler<CreateBudgetCommand, Guid>
{
    public async Task<Guid> Handle(CreateBudgetCommand request, CancellationToken ct)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);
        if (account == null || account.User.Id != currentUserService.UserId)
            throw new NotFoundException(typeof(Account), request.AccountId);

        var money = new Money(request.InitialBalance, request.Currency);
        var budget = new Budget(request.Name, money);

        account.AddBudget(budget);

        await accountRepository.UpdateAsync(account, ct);

        return budget.Id;
    }
}
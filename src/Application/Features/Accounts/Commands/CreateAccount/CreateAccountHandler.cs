using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountHandler : IRequestHandler<CreateAccountCommand, Guid>
{
    private readonly IAccountRepository accountRepository;
    private readonly IUserRepository userRepository;

    public CreateAccountHandler(IAccountRepository accountRepository, IUserRepository userRepository)
    {
        this.accountRepository = accountRepository;
        this.userRepository = userRepository;
    }

    public async Task<Guid> Handle(CreateAccountCommand request, CancellationToken cancellationToken)
    {
        // 1. Проверяем существование пользователя
        var user = await userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            throw new NotFoundException(typeof(User), request.UserId);
        }

        // 2. Проверяем, нет ли уже аккаунта (опционально)
        var existingAccounts = await accountRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (existingAccounts.Any())
        {
            throw new ConflictException("User already has an account.");
        }

        // 3. Создаем сущность
        var account = new Account(user);

        // 4. Сохраняем
        await accountRepository.AddAsync(account, cancellationToken);

        return account.Id;
    }
}
using FluentValidation;

namespace KontoApi.Application.Features.Accounts.Commands.CreateAccount;

public class CreateAccountValidator : AbstractValidator<CreateAccountCommand>
{
    public CreateAccountValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

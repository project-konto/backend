using FluentValidation;
using KontoApi.Application.DTOs;

namespace KontoApi.Api.Validators;

public class CreateTransactionRequestValidator : AbstractValidator<CreateTransactionRequest>
{
	public CreateTransactionRequestValidator()
	{
		// Rule: amount > 0
		RuleFor(x => x.Amount)
			.GreaterThan(0)
			.WithMessage("Amount must be greater than zero");

		// Rule: currency is an ISO code
		RuleFor(x => x.Currency)
			.NotEmpty()
			.Length(3)
			.WithMessage("Currency must be a valid ISO currency code");

		// Rule: Category is not empty
		RuleFor(x => x.Category)
			.NotEmpty()
			.WithMessage("Category is required");

		// Rule: Type is one of available
		RuleFor(x => x.Type)
			.NotEmpty()
			.Must(type => type is "Income" or "Expense" or "Transfer")
			.WithMessage("Type is required");
	}
}
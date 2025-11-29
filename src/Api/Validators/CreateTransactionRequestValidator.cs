using FluentValidation;
using KontoApi.Application.DTOs;

namespace KontoApi.Api.Validators;

public class CreateTransactionRequestValidator: AbstractValidator<CreateTransactionRequest>
{
    public CreateTransactionRequestValidator()
    {

        // Правило: Сумма должна быть больше 0
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");
        
        // Правило: Валюта не должна быть пустой и должна быть длиной 3 символа (USD, EUR, RUB)
        RuleFor(x => x.Currency)
            .NotEmpty()
            .Length(3);

        // Правило: Категория не должна быть пустой
        RuleFor(x => x.Category)
            .NotEmpty()
            .WithMessage("Category is required");

        // Правило: Тип должен определённый и не пустой
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(type => type == "Income" || type == "Expense" || type == "Transfer")
            .WithMessage("Type is required");

    }
}
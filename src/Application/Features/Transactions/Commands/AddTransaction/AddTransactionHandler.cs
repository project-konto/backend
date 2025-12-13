using KontoApi.Application.Exceptions;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Transactions.Commands.AddTransaction;

public class AddTransactionHandler : IRequestHandler<AddTransactionCommand, Guid>
{
    private readonly IBudgetRepository budgetRepository;
    private readonly IApplicationDbContext dbContext;

    public AddTransactionHandler(IBudgetRepository budgetRepository, IApplicationDbContext dbContext)
    {
        this.budgetRepository = budgetRepository;
        this.dbContext = dbContext;
    }

    public async Task<Guid> Handle(AddTransactionCommand request, CancellationToken ct)
    {
        var budget = await budgetRepository.GetByIdAsync(request.BudgetId, ct);
        if (budget == null) throw new NotFoundException(typeof(Budget), request.BudgetId);

        var category = await dbContext.Categories.FindAsync([request.CategoryId], ct);
        if (category == null) throw new NotFoundException(typeof(Category), request.CategoryId);

        var money = new Money(request.Amount, request.Currency);
        var transaction = new Transaction(money, request.Type, category, request.Date, request.Description);

        budget.AddTransaction(transaction);

        await budgetRepository.UpdateAsync(budget, ct);

        return transaction.Id;
    }
}
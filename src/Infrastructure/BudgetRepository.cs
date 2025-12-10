using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;
namespace KontoApi.Infrastructure;

public class BudgetRepository(KontoDbContext context) : IBudgetRepository
{
    public async Task<Budget?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await context.Budget.FindAsync(userId, cancellationToken);

        if (user == null)
            return null;

        return await BudgetDto(user);
    }

    private static async Task<Budget?> BudgetDto(Models.BudgetEntity budget)
    {
        var dto = new Budget(budget.Name, new Money(budget.CurrentBalance, budget.Currency));

        return dto;
    }
}
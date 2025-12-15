using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Persistence.Repositories;

public class CategoryRepository(KontoDbContext context) : ICategoryRepository
{
    public async Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken) =>
        await context.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken) =>
        await context.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

    public async Task AddAsync(Category category, CancellationToken cancellationToken)
    {
        await context.Categories.AddAsync(category, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Category category, CancellationToken cancellationToken)
    {
        context.Categories.Update(category);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        if (entity == null)
            return;

        context.Categories.Remove(entity);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        var normalizedName = name.Trim();
        return await context.Categories
            .AnyAsync(c => c.Name == normalizedName, cancellationToken);
    }
}
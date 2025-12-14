using KontoApi.Domain;

namespace KontoApi.Application.Common.Interfaces;

public interface ICategoryRepository
{
    // Used by GetCategoriesHandler
    Task<IReadOnlyCollection<Category>> GetAllAsync(CancellationToken cancellationToken);

    // Used by RenameCategoryHandler, DeleteCategoryHandler
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken);

    // Used by CreateCategoryHandler
    Task AddAsync(Category category, CancellationToken cancellationToken);

    // Used by RenameCategoryHandler (This was missing in your version)
    Task UpdateAsync(Category category, CancellationToken cancellationToken);

    // Used by DeleteCategoryHandler
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);

    // Useful for CreateCategoryValidator (to prevent duplicates)
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
}

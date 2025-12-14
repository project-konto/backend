using KontoApi.Domain;
using KontoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure;

public static class CategorySeeder
{
    private static readonly string[] DefaultNames =
    [
        "rent",
        "taxes",
        "car",
        "education",
        "groceries",
        "restaurants",
        "beauty",
        "sport",
        "clothes",
        "cash"
    ];

    public static async Task SeedAsync(KontoDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var name in DefaultNames)
        {
            var exists = await context.Categories.AnyAsync(c => c.Name == name, cancellationToken);
            if (!exists)
                context.Categories.Add(new Category(name));
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}

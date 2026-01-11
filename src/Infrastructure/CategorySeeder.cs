using KontoApi.Domain;
using KontoApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure;

public static class CategorySeeder
{
    private static readonly Dictionary<string, string> EnglishToRussian = new()
    {
        { "rent", "Аренда" },
        { "taxes", "Налоги" },
        { "car", "Транспорт" },
        { "education", "Образование" },
        { "groceries", "Супермаркеты" },
        { "restaurants", "Рестораны и кафе" },
        { "beauty", "Красота" },
        { "sport", "Спорт" },
        { "clothes", "Одежда" },
        { "cash", "Переводы" },
        { "Uncategorized", "Прочее" },
    };

    public static async Task SeedAsync(KontoDbContext context, CancellationToken cancellationToken = default)
    {
        foreach (var pair in EnglishToRussian)
        {
            var engName = pair.Key;
            var rusName = pair.Value;

            // Deduplicate Russian categories if they exist multiple times
            var existingRus = await context.Categories
                .Where(c => c.Name == rusName)
                .ToListAsync(cancellationToken);

            if (existingRus.Count > 1)
            {
                var primary = existingRus[0];
                var duplicates = existingRus.Skip(1).ToList();

                foreach (var dup in duplicates)
                {
                    var txs = await context.Transactions
                        .Include(t => t.TransactionCategory)
                        .Where(t => t.TransactionCategory.Id == dup.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var tx in txs)
                    {
                        tx.UpdateCategory(primary);
                    }
                    
                    context.Categories.Remove(dup);
                }
                await context.SaveChangesAsync(cancellationToken);
            }

            var engCat = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == engName, cancellationToken);
            
            var rusCat = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == rusName, cancellationToken);

            if (engCat != null)
            {
                if (rusCat != null)
                {
                    // If both exist: move transactions from English to Russian, then delete English
                    var transactions = await context.Transactions
                        .Include(t => t.TransactionCategory)
                        .Where(t => t.TransactionCategory.Id == engCat.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var tx in transactions)
                    {
                        tx.UpdateCategory(rusCat);
                    }

                    context.Categories.Remove(engCat);
                }
                else
                {
                    // Only English exists: rename to Russian
                    engCat.Rename(rusName);
                }
            }
            else
            {
                // English does not exist
                if (rusCat == null)
                {
                    // Neither exist: create Russian
                    context.Categories.Add(new Category(rusName));
                }
                // If only Russian exists: do nothing
            }

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Tests.CategoryTests;

public class CategorySeederTests
{
    [Fact]
    public async Task AddDefaultCategories()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;
        await CategorySeeder.SeedAsync(context);

        var categories = await context.Categories.OrderBy(c => c.Name).ToListAsync();
        Assert.Equal(10, categories.Count);
    }

    [Fact]
    public async Task NotCreateDuplicates()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;
        await CategorySeeder.SeedAsync(context);
        await CategorySeeder.SeedAsync(context);
        
        var count = await context.Categories.CountAsync();
        Assert.Equal(10, count);
    }

    [Fact]
    public async Task AddOnlyMissingOnes()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        context.Categories.Add(new Domain.Category("rent"));
        context.Categories.Add(new Domain.Category("taxes"));
        
        await context.SaveChangesAsync();
        await CategorySeeder.SeedAsync(context);
        
        var count = await context.Categories.CountAsync();
        Assert.Equal(10, count); 
    }
}
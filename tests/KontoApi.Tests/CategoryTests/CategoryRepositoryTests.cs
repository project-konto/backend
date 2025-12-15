using KontoApi.Domain;
using KontoApi.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Tests.CategoryTests;

public class CategoryRepositoryTests
{
    [Fact]
    public async Task WhenNotExistsFalse()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        var exists = await repository.ExistsByNameAsync("rent", CancellationToken.None);

        Assert.False(exists);
    }

    [Fact]
    public async Task WhenExistsTrue()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        await repository.AddAsync(new Category("rent"), CancellationToken.None);

        var exists = await repository.ExistsByNameAsync("rent", CancellationToken.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task TrimName()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        await repository.AddAsync(new Category("rent"), CancellationToken.None);

        var exists = await repository.ExistsByNameAsync("  rent  ", CancellationToken.None);
        Assert.True(exists);
    }

    [Fact]
    public async Task ReturnsSortedByName()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        await repository.AddAsync(new Category("taxes"), CancellationToken.None);
        await repository.AddAsync(new Category("clothes"), CancellationToken.None);
        await repository.AddAsync(new Category("rent"), CancellationToken.None);

        var all = await repository.GetAllAsync(CancellationToken.None);
        var names = all.Select(x => x.Name).ToList();

        Assert.Equal(new[] { "clothes", "rent", "taxes" }, names);
    }

    [Fact]
    public async Task RemovesEntity()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        var category = new Category("rent");

        context.Categories.Add(category);
        await context.SaveChangesAsync();
        await repository.DeleteAsync(category.Id, CancellationToken.None);

        var inDataBase = await context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
        Assert.Null(inDataBase);
    }

    [Fact]
    public async Task NotThrowsExceptionWhenNotExists()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        var exception = await Record.ExceptionAsync(() =>
            repository.DeleteAsync(Guid.NewGuid(), CancellationToken.None));

        Assert.Null(exception);
    }

    [Fact]
    public async Task ReturnsCategory()
    {
        var (context, connection) = DbContextFactory.CreateSqliteInMemory();
        await using var _ = connection;

        var repository = new CategoryRepository(context);
        var category = new Category("rent");

        context.Categories.Add(category);
        await context.SaveChangesAsync();

        var result = await repository.GetByIdAsync(category.Id, CancellationToken.None);
        Assert.NotNull(result);
        Assert.Equal(category.Id, result.Id);
        Assert.Equal("rent", result.Name);
    }
}
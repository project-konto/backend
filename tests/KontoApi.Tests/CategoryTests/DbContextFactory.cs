using KontoApi.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Tests.CategoryTests;

public class DbContextFactory
{
    public static (KontoDbContext context, SqliteConnection connection) CreateSqliteInMemory()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<KontoDbContext>()
            .UseSqlite(connection)
            .EnableSensitiveDataLogging()
            .Options;

        var context = new KontoDbContext(options);
        context.Database.EnsureCreated();

        return (context, connection);
    }
}

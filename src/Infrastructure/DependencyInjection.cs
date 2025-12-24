using KontoApi.Application.Common.Interfaces;
using KontoApi.Infrastructure.Auth;
using KontoApi.Infrastructure.Persistence;
using KontoApi.Infrastructure.Persistence.Repositories;
using KontoApi.Infrastructure.Repositories;
using KontoApi.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace KontoApi.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            // InMemory provider as fallback for testing
            services.AddDbContext<KontoDbContext>(options =>
                options.UseInMemoryDatabase("KontoApi_TestDb"));
        }
        else
        {
            services.AddDbContext<KontoDbContext>(options =>
            {
                options.UseNpgsql(connectionString);

                if (!environment.IsDevelopment()) return;

                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });
        }

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<KontoDbContext>());

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IBudgetRepository, BudgetRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IStatementParser, StatementParser>();

        services.Configure<JwtSettings>(configuration.GetSection("Jwt"));
        services.AddSingleton<IJwtProvider, JwtProvider>();

        return services;
    }
}
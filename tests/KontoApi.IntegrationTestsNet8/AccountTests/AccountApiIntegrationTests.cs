using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using KontoApi.Infrastructure.Persistence;
using Xunit;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;
using KontoApi.TestEntryPoint;

namespace KontoApi.IntegrationTestsNet8.AccountTests;

public class AccountApiIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public AccountApiIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        // Use a unique in-memory database per test class to avoid state leakage between tests
        var uniqueFactory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<KontoDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);
                services.AddDbContext<KontoDbContext>(options =>
                    options.UseInMemoryDatabase($"KontoApi_TestDb_{Guid.NewGuid()}"));

                // Build a temporary provider to seed the test user into this new in-memory database
                var sp = services.BuildServiceProvider();
                using (var seedScope = sp.CreateScope())
                {
                    var db = seedScope.ServiceProvider.GetRequiredService<KontoDbContext>();
                    var hasher = seedScope.ServiceProvider.GetRequiredService<KontoApi.Application.Common.Interfaces.IPasswordHasher>();
                    var testEmail = "testuser@example.com";
                    if (!db.Users.Any(u => u.Email == testEmail))
                    {
                        var user = new KontoApi.Domain.User("Test User", testEmail, hasher.Hash("Test123!"));
                        db.Users.Add(user);
                        db.SaveChanges();
                    }
                }
            });
        });

        client = uniqueFactory.CreateClient();

        // After host start, seed test user into the server's service provider to ensure TestAuthenticationHandler can find it
        using (var scope = uniqueFactory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<KontoDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<KontoApi.Application.Common.Interfaces.IPasswordHasher>();
            var testEmail = "testuser@example.com";
            if (!db.Users.Any(u => u.Email == testEmail))
            {
                var user = new KontoApi.Domain.User("Test User", testEmail, hasher.Hash("Test123!"));
                db.Users.Add(user);
                db.SaveChanges();
            }
        }
    }

    [Fact]
    public async Task CreateAccount_WithInvalidData_ReturnsBadRequest()
    {
        var request = new { Name = "" };
        var response = await client.PostAsJsonAsync("/api/account", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetAccount_WhenNotExists_ReturnsNotFound()
    {
        // Ensure any pre-existing account is removed (tests share an in-memory DB).
        await client.DeleteAsync("/api/account");

        var response = await client.GetAsync("/api/account");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreatedAndAccountId()
    {
        var request = new { Name = "Test account" };
        var response = await client.PostAsJsonAsync("/api/account", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateAccountResponse>();
        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.AccountId);
    }

    [Fact]
    public async Task GetAccount_ReturnsOkAndAccountData()
    {
        var createRequest = new { Name = "Test account" };
        var createResponse = await client.PostAsJsonAsync("/api/account", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("/api/account");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var account = await getResponse.Content.ReadFromJsonAsync<AccountOverviewDto>();
        Assert.NotNull(account);
        // The API currently returns the account owner's name in the Name field
        Assert.Equal("Test User", account.Name);
    }

    [Fact]
    public async Task DeleteAccount_ThenGet_ReturnsNotFound()
    {
        var createRequest = new { Name = "ToDelete" };
        var createResponse = await client.PostAsJsonAsync("/api/account", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var deleteResponse = await client.DeleteAsync("/api/account");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await client.GetAsync("/api/account");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    public class CreateAccountResponse
    {
        public Guid AccountId { get; set; }
    }
}
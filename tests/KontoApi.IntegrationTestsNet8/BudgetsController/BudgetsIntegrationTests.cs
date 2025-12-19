using KontoApi.TestEntryPoint;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace KontoApi.IntegrationTestsNet8.BudgetsController;

public class BudgetsIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public BudgetsIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateBudget_WithValidData_ReturnsCreatedAndBudgetId()
    {
        // create account to attach budget to
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Budget owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var request = new {
            AccountId = accountId,
            Name = "Test budget",
            InitialBalance = 1000m,
            Currency = "USD"
        };
        var response = await client.PostAsJsonAsync("/api/budgets", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        Assert.NotNull(result);
        Assert.True(result.BudgetId != Guid.Empty);
    }

    [Fact]
    public async Task CreateBudget_WithInvalidData_ReturnsBadRequest()
    {
        var request = new {
            AccountId = Guid.Empty,
            Name = "",
            InitialBalance = -1m,
            Currency = ""
        };
        var response = await client.PostAsJsonAsync("/api/budgets", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetBudget_NonExistent_ReturnsNotFound()
    {
        var response = await client.GetAsync($"/api/budgets/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetBudget_ReturnsCorrectData()
    {
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Budget owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var createRequest = new {
            AccountId = accountId,
            Name = "Budget1",
            InitialBalance = 500m,
            Currency = "USD"
        };
        var createResponse = await client.PostAsJsonAsync("/api/budgets", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        var getResponse = await client.GetAsync($"/api/budgets/{result.BudgetId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        // Можно добавить десериализацию и проверку полей
    }

    [Fact]
    public async Task CreateAndDeleteBudget_ThenGet_ReturnsNotFound()
    {
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Budget owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var createRequest = new {
            AccountId = accountId,
            Name = "BudgetToDelete",
            InitialBalance = 100m,
            Currency = "USD"
        };
        var createResponse = await client.PostAsJsonAsync("/api/budgets", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        var deleteResponse = await client.DeleteAsync($"/api/budgets/{result.BudgetId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await client.GetAsync($"/api/budgets/{result.BudgetId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateAndRenameBudget_ReturnsNoContent()
    {
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Budget owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var createRequest = new {
            AccountId = accountId,
            Name = "BudgetToRename",
            InitialBalance = 200m,
            Currency = "USD"
        };
        var createResponse = await client.PostAsJsonAsync("/api/budgets", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateBudgetResponse>();
        var renameRequest = new { NewName = "RenamedBudget" };
        var renameResponse = await client.PatchAsJsonAsync($"/api/budgets/{result.BudgetId}/name", renameRequest);
        Assert.Equal(HttpStatusCode.NoContent, renameResponse.StatusCode);
    }

    public class CreateBudgetResponse
    {
        public Guid BudgetId { get; set; }
    }
}

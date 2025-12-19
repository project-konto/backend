using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace KontoApi.IntegrationTestsNet8.TransactionController;

using KontoApi.TestEntryPoint;

public class TransactionsIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public TransactionsIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task AddTransaction_WithValidData_ReturnsCreatedAndTransactionId()
    {
        // create account and budget first
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Tx owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var budgetCreate = new { AccountId = accountId, Name = "TxBudget", InitialBalance = 0m, Currency = "USD" };
        var budgetResponse = await client.PostAsJsonAsync("/api/budgets", budgetCreate);
        budgetResponse.EnsureSuccessStatusCode();
        var budgetJson = await budgetResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid budgetId;
        if (budgetJson.TryGetProperty("budgetId", out var bprop1))
            budgetId = bprop1.GetGuid();
        else if (budgetJson.TryGetProperty("BudgetId", out var bprop2))
            budgetId = bprop2.GetGuid();
        else
            budgetId = Guid.Parse(budgetJson.GetRawText().Trim('"'));

        var categoryResp = await client.PostAsJsonAsync("/api/categories", new { Name = "txcat" });
        categoryResp.EnsureSuccessStatusCode();
        var categoryJson = await categoryResp.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid categoryIdForTest;
        if (categoryJson.TryGetProperty("categoryId", out var cprop1))
            categoryIdForTest = cprop1.GetGuid();
        else if (categoryJson.TryGetProperty("CategoryId", out var cprop2))
            categoryIdForTest = cprop2.GetGuid();
        else
            categoryIdForTest = Guid.Parse(categoryJson.GetRawText().Trim('"'));

        var request = new
        {
            BudgetId = budgetId,
            Amount = 100.0m,
            Currency = "USD",
            Type = 1, // Expense
            CategoryId = categoryIdForTest,
            Date = DateTime.UtcNow,
            Description = "Test transaction"
        };
        var response = await client.PostAsJsonAsync("/api/transactions", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<CreateTransactionResponse>();
        Assert.NotNull(result);
        Assert.True(result.TransactionId != Guid.Empty);
    }

    [Fact]
    public async Task AddTransaction_WithInvalidData_ReturnsBadRequest()
    {
        var request = new
        {
            BudgetId = Guid.Empty,
            Amount = -1.0m,
            Currency = "",
            Type = "",
            CategoryId = Guid.Empty,
            Date = DateTime.UtcNow,
            Description = ""
        };
        var response = await client.PostAsJsonAsync("/api/transactions", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetTransactionById_NonExistent_ReturnsNotFound()
    {
        var response = await client.GetAsync($"/api/transactions/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddAndGetTransaction_ReturnsCorrectData()
    {
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Tx owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountJson = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountJson.TryGetProperty("accountId", out var prop1))
            accountId = prop1.GetGuid();
        else if (accountJson.TryGetProperty("AccountId", out var prop2))
            accountId = prop2.GetGuid();
        else
            accountId = Guid.Parse(accountJson.GetRawText().Trim('"'));

        var budgetCreate = new { AccountId = accountId, Name = "TxBudget", InitialBalance = 0m, Currency = "USD" };
        var budgetResponse = await client.PostAsJsonAsync("/api/budgets", budgetCreate);
        budgetResponse.EnsureSuccessStatusCode();
        var budgetJson = await budgetResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid budgetId;
        if (budgetJson.TryGetProperty("budgetId", out var bprop1))
            budgetId = bprop1.GetGuid();
        else if (budgetJson.TryGetProperty("BudgetId", out var bprop2))
            budgetId = bprop2.GetGuid();
        else
            budgetId = Guid.Parse(budgetJson.GetRawText().Trim('"'));

        var categoryResp2 = await client.PostAsJsonAsync("/api/categories", new { Name = "txcat2" });
        categoryResp2.EnsureSuccessStatusCode();
        var categoryJson2 = await categoryResp2.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid categoryId2;
        if (categoryJson2.TryGetProperty("categoryId", out var c2p1))
            categoryId2 = c2p1.GetGuid();
        else if (categoryJson2.TryGetProperty("CategoryId", out var c2p2))
            categoryId2 = c2p2.GetGuid();
        else
            categoryId2 = Guid.Parse(categoryJson2.GetRawText().Trim('"'));

        var createRequest = new
        {
            BudgetId = budgetId,
            Amount = 50.0m,
            Currency = "USD",
            Type = 0, // Income
            CategoryId = categoryId2,
            Date = DateTime.UtcNow,
            Description = "Income transaction"
        };
        var createResponse = await client.PostAsJsonAsync("/api/transactions", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateTransactionResponse>();
        var getResponse = await client.GetAsync($"/api/transactions/{result.TransactionId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task AddAndDeleteTransaction_ThenGet_ReturnsNotFound()
    {
        var accountResponse = await client.PostAsJsonAsync("/api/account", new { Name = "Tx owner" });
        accountResponse.EnsureSuccessStatusCode();
        var accountResult = await accountResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid accountId;
        if (accountResult.TryGetProperty("accountId", out var propA))
            accountId = propA.GetGuid();
        else if (accountResult.TryGetProperty("AccountId", out var propA2))
            accountId = propA2.GetGuid();
        else
            accountId = Guid.Parse(accountResult.GetRawText().Trim('"'));

        var budgetCreate = new { AccountId = accountId, Name = "TxBudget", InitialBalance = 0m, Currency = "USD" };
        var budgetResponse = await client.PostAsJsonAsync("/api/budgets", budgetCreate);
        budgetResponse.EnsureSuccessStatusCode();
        var budgetResult = await budgetResponse.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid budgetId;
        if (budgetResult.TryGetProperty("budgetId", out var propB))
            budgetId = propB.GetGuid();
        else if (budgetResult.TryGetProperty("BudgetId", out var propB2))
            budgetId = propB2.GetGuid();
        else
            budgetId = Guid.Parse(budgetResult.GetRawText().Trim('"'));

        var categoryResp3 = await client.PostAsJsonAsync("/api/categories", new { Name = "txcat3" });
        categoryResp3.EnsureSuccessStatusCode();
        var categoryJson3 = await categoryResp3.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
        Guid categoryId3;
        if (categoryJson3.TryGetProperty("categoryId", out var c3p1))
            categoryId3 = c3p1.GetGuid();
        else if (categoryJson3.TryGetProperty("CategoryId", out var c3p2))
            categoryId3 = c3p2.GetGuid();
        else
            categoryId3 = Guid.Parse(categoryJson3.GetRawText().Trim('"'));

        var createRequest = new
        {
            BudgetId = budgetId,
            Amount = 10.0m,
            Currency = "USD",
            Type = 1, // Expense
            CategoryId = categoryId3,
            Date = DateTime.UtcNow,
            Description = "To delete"
        };
        var createResponse = await client.PostAsJsonAsync("/api/transactions", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateTransactionResponse>();
        var deleteResponse = await client.DeleteAsync($"/api/transactions/{result.TransactionId}?budgetId={budgetId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await client.GetAsync($"/api/transactions/{result.TransactionId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    public class CreateTransactionResponse
    {
        public Guid TransactionId { get; set; }
    }
}

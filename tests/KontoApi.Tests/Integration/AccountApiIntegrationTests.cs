using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using KontoApi.Domain;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Mvc;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;


namespace KontoApi.Tests.Integration;

public class AccountApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient client;

    public AccountApiIntegrationTests(WebApplicationFactory<Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task CreateAccount_ReturnsCreated()
    {
        var request = new { Name = "Test account" };
        var response = await client.PostAsJsonAsync("/api/account", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetAccount_ReturnsOkAndAccountData()
    {
        var createRequest = new { Name = "Test account" };
        var createResponse = await client.PostAsJsonAsync("/api/account", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("api/account");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task GetAccount_ReturnsCorrectAccountData()
    {
        var createRequest = new { Name = "Test account" };
        var createResponse = await client.PostAsJsonAsync("/api/account", createRequest);
        createResponse.EnsureSuccessStatusCode();

        var getResponse = await client.GetAsync("api/account");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var account = await getResponse.Content.ReadFromJsonAsync<AccountOverviewDto>();
        Assert.NotNull(account);
        Assert.Equal("Test account", account.Name);
    }
}
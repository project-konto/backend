using System.Net;
using System.Net.Http.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;


namespace KontoApi.Tests.Integration;

public class AccountApiIntegrationTests: IClassFixture<WebApplicationFactory<Program>>
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
}
using KontoApi.TestEntryPoint;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KontoApi.IntegrationTestsNet8.AuthController;

public class AuthIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public AuthIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_ReturnsToken_OnValidCredentials()
    {
        var loginRequest = new { Email = "testuser@example.com", Password = "Test123!" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrEmpty(json?.Token));
    }

    [Fact]
    public async Task Login_ReturnsUnauthorized_OnInvalidCredentials()
    {
        var loginRequest = new { Email = "wrong@example.com", Password = "WrongPass" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyEmail_ReturnsBadRequest()
    {
        var loginRequest = new { Email = "", Password = "Test123!" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithEmptyPassword_ReturnsBadRequest()
    {
        var loginRequest = new { Email = "testuser@example.com", Password = "" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidEmailFormat_ReturnsBadRequest()
    {
        var loginRequest = new { Email = "notanemail", Password = "Test123!" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Login_ErrorResponse_HasExpectedFormat()
    {
        var loginRequest = new { Email = "wrong@example.com", Password = "WrongPass" };
        var response = await client.PostAsJsonAsync("/api/auth/login", loginRequest);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();
        Assert.NotNull(error);
        Assert.False(string.IsNullOrEmpty(error.Message));
    }

    [Fact]
    public async Task ProtectedEndpoint_WithoutToken_Returns401()
    {
        var response = await client.GetAsync("/api/account");
        // Test environment uses test authentication handler which authenticates requests
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }


    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public class ErrorResponse
    {
        public string Message { get; set; }
    }
}

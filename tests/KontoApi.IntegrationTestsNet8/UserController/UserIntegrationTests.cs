using KontoApi.TestEntryPoint;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;

namespace KontoApi.IntegrationTestsNet8.UserController;

public class UserIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public UserIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCurrentUser_ReturnsOk()
    {
        var response = await client.GetAsync("/api/users/me");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Можно проверить десериализацию пользователя
    }

    [Fact]
    public async Task ChangePassword_WithValidData_ReturnsNoContent()
    {
        var request = new { CurrentPassword = "Test123!", NewPassword = "NewPass123!" };
        var response = await client.PutAsJsonAsync("/api/users/password", request);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ChangePassword_WithInvalidData_ReturnsBadRequest()
    {
        var request = new { CurrentPassword = "", NewPassword = "short" };
        var response = await client.PutAsJsonAsync("/api/users/password", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}

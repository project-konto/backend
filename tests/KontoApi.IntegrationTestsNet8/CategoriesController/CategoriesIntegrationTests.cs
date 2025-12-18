using KontoApi.TestEntryPoint;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using System;

namespace KontoApi.IntegrationTestsNet8.CategoriesController;

public class CategoriesIntegrationTests : IClassFixture<WebApplicationFactory<KontoApi.TestEntryPoint.Program>>
{
    private readonly HttpClient client;

    public CategoriesIntegrationTests(WebApplicationFactory<KontoApi.TestEntryPoint.Program> factory)
    {
        client = factory.CreateClient();
    }

    [Fact]
    public async Task GetCategories_ReturnsOk()
    {
        var response = await client.GetAsync("/api/categories");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithValidData_ReturnsCreated()
    {
        var request = new { Name = "Test category" };
        var response = await client.PostAsJsonAsync("/api/categories", request);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task CreateCategory_WithInvalidData_ReturnsBadRequest()
    {
        var request = new { Name = "" };
        var response = await client.PostAsJsonAsync("/api/categories", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task GetCategoryById_NonExistent_ReturnsNotFound()
    {
        var response = await client.GetAsync($"/api/categories/{Guid.NewGuid()}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateAndGetCategoryById_ReturnsCorrectData()
    {
        var createRequest = new { Name = "Category1" };
        var createResponse = await client.PostAsJsonAsync("/api/categories", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var getResponse = await client.GetAsync($"/api/categories/{result.CategoryId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateAndDeleteCategory_ThenGet_ReturnsNotFound()
    {
        var createRequest = new { Name = "ToDelete" };
        var createResponse = await client.PostAsJsonAsync("/api/categories", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var deleteResponse = await client.DeleteAsync($"/api/categories/{result.CategoryId}");
        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        var getResponse = await client.GetAsync($"/api/categories/{result.CategoryId}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task CreateAndRenameCategory_ReturnsNoContent()
    {
        var createRequest = new { Name = "ToRename" };
        var createResponse = await client.PostAsJsonAsync("/api/categories", createRequest);
        createResponse.EnsureSuccessStatusCode();
        var result = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var renameRequest = new { NewName = "RenamedCategory" };
        var renameResponse = await client.PatchAsJsonAsync($"/api/categories/{result.CategoryId}/name", renameRequest);
        Assert.Equal(HttpStatusCode.NoContent, renameResponse.StatusCode);
    }

    public class CreateCategoryResponse
    {
        public Guid CategoryId { get; set; }
    }
}

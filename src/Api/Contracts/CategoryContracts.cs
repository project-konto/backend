namespace KontoApi.Api.Contracts;

public class CreateCategoryRequest
{
    public string Name { get; set; } = "";
}

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
}
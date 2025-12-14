using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoryByIdQuery : IRequest<CategoryDto>
{
    public Guid Id { get; set; }
}

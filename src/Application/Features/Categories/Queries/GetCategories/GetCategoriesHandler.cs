using KontoApi.Application.Common.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToList();
    }
}
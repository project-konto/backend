using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    private readonly ICategoryRepository categoryRepository;

    public GetCategoriesHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);

        return categories
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToList();
    }
}

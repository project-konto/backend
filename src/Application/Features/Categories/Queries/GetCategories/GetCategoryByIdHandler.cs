using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoryByIdHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        return category == null
            ? throw new NotFoundException($"Category with id {request.Id} not found")
            : new(category.Id, category.Name);
    }
}
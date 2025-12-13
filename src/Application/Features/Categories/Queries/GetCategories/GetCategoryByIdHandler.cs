using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Exceptions;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoryByIdHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly ICategoryRepository categoryRepository;
    
    public GetCategoryByIdHandler(ICategoryRepository categoryRepository) =>
        this.categoryRepository = categoryRepository;

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new NotFoundException($"Category with id {request.Id} not found");
        
        return new CategoryDto(category.Id, category.Name);
    }
}
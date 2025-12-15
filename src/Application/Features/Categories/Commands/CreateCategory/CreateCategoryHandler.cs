using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, Guid>
{
    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name);

        await categoryRepository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly ICategoryRepository categoryRepository;

    public CreateCategoryHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category(request.Name);

        await categoryRepository.AddAsync(category, cancellationToken);

        return category.Id;
    }
}
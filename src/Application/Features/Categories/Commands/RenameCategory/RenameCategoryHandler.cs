using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Exceptions;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.RenameCategory;

public class RenameCategoryHandler : IRequestHandler<RenameCategoryCommand>
{
    private readonly ICategoryRepository categoryRepository;

    public RenameCategoryHandler(ICategoryRepository categoryRepository)
    {
        this.categoryRepository = categoryRepository;
    }

    public async Task Handle(RenameCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
        {
            throw new NotFoundException(typeof(Category), request.Id);
        }

        category.Rename(request.NewName);

        await categoryRepository.UpdateAsync(category, cancellationToken);
    }
}
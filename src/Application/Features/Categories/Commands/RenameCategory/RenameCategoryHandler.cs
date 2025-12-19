using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.RenameCategory;

public class RenameCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<RenameCategoryCommand>
{
    public async Task Handle(RenameCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category == null)
            throw new NotFoundException(typeof(Category), request.Id);

        category.Rename(request.NewName);

        await categoryRepository.UpdateAsync(category, cancellationToken);
    }
}
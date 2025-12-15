using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryHandler(ICategoryRepository categoryRepository) : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category == null)
            throw new NotFoundException(typeof(Category), request.Id);

        await categoryRepository.DeleteAsync(request.Id, cancellationToken);
    }
}

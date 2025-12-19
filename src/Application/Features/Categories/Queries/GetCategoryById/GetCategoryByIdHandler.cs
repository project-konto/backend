using KontoApi.Application.Common.Exceptions;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Features.Categories.DTOs;
using KontoApi.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        return category == null
            ? throw new NotFoundException(typeof(Category), request.Id)
            : new(category.Id, category.Name);
    }
}
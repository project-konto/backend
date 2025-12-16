using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Features.Categories.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public class GetCategoriesHandler(IApplicationDbContext dbContext)
    : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name))
            .ToListAsync(cancellationToken);
    }
}
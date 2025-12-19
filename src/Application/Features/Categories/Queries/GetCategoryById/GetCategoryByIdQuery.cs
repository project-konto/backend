using KontoApi.Application.Features.Categories.DTOs;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategoryById;

public record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;
using KontoApi.Application.Features.Categories.DTOs;
using MediatR;

namespace KontoApi.Application.Features.Categories.Queries.GetCategories;

public record GetCategoriesQuery : IRequest<List<CategoryDto>>;
using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name) : IRequest<Guid>;

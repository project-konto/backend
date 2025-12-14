using MediatR;

namespace KontoApi.Application.Features.Categories.Commands.RenameCategory;

public record RenameCategoryCommand(Guid Id, string NewName) : IRequest;

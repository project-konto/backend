using FluentValidation;

namespace KontoApi.Application.Features.Categories.Commands.RenameCategory;

public class RenameCategoryValidator : AbstractValidator<RenameCategoryCommand>
{
    public RenameCategoryValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.NewName).NotEmpty().MaximumLength(80);
    }
}

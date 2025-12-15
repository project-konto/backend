using KontoApi.Application.Features.Categories.Commands.CreateCategory;
using KontoApi.Application.Features.Categories.Commands.DeleteCategory;
using KontoApi.Application.Features.Categories.Commands.RenameCategory;
using KontoApi.Application.Features.Categories.Queries.GetCategories;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken) =>
        Ok(await Mediator.Send(new GetCategoriesQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken) =>
        Ok(await Mediator.Send(new GetCategoryByIdQuery { Id = id }, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command,
        CancellationToken cancellationToken)
    {
        var created = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = created }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Rename(Guid id, [FromBody] RenameCategoryCommand command,
        CancellationToken cancellationToken)
    {
        await Mediator.Send(command with { Id = id }, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}
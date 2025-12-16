using KontoApi.Api.Contracts; // For ErrorResponse
using KontoApi.Application.Features.Categories.Commands.CreateCategory;
using KontoApi.Application.Features.Categories.Commands.DeleteCategory;
using KontoApi.Application.Features.Categories.Commands.RenameCategory;
using KontoApi.Application.Features.Categories.DTOs;
using KontoApi.Application.Features.Categories.Queries.GetCategories;
using KontoApi.Application.Features.Categories.Queries.GetCategoryById;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class CategoriesController : BaseController // 2. Plural Name
{
    // GET api/categories
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetCategoriesQuery(), cancellationToken));

    // GET api/categories/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await Mediator.Send(new GetCategoryByIdQuery(id), cancellationToken));

    // POST api/categories
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateCategoryCommand(request.Name);
        var categoryId = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = categoryId }, new { CategoryId = categoryId });
    }

    // PATCH api/categories/{id}/name
    [HttpPatch("{id:guid}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rename(
        Guid id,
        [FromBody] RenameCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RenameCategoryCommand(id, request.NewName);
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    // DELETE api/categories/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}

public record CreateCategoryRequest(string Name);

public record RenameCategoryRequest(string NewName);
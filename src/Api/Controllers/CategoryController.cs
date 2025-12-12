using KontoApi.Api.Contracts;
using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

public class CategoryController(ICategoryRepository categoryRepository) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var categories = await categoryRepository.GetAllAsync(cancellationToken);
        return Ok(categories.Select(c => new CategoryResponse
        {
            Id = c.Id,
            Name = c.Name,
        }));
    }

    [HttpPost]
    public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest("Category name is required");

        var exists = await categoryRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (exists)
            return Conflict("Category already exists");

        var category = new Category(request.Name);
        await categoryRepository.AddAsync(category, cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { id = category.Id }, new CategoryResponse
        {
            Id = category.Id,
            Name = category.Name,
        });
    }

    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await categoryRepository.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
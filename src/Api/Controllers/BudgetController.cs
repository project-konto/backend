using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Budgets.Commands.CreateBudget;
using KontoApi.Application.Features.Budgets.Commands.DeleteBudget;
using KontoApi.Application.Features.Budgets.Commands.RenameBudget;
using KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BudgetController : BaseController
{
    // POST api/budget/create
    [HttpPost("create")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBudget(CreateBudgetCommand command)
    {
        var budgetId = await Mediator.Send(command);
        return Ok(new { BudgetId = budgetId });
    }

    // GET api/budget/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudget(Guid id)
    {
        var query = new GetBudgetDetailsQuery(id);
        var result = await Mediator.Send(query);
        return Ok(result);
    }

    // DELETE api/budget/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget(Guid budgetId)
    {
        await Mediator.Send(new DeleteBudgetCommand(BudgetId: budgetId));
        return NoContent();
    }


    // PUT api/budget/{id}/rename
    [HttpPut("{id:guid}/rename")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameBudget(Guid id, [FromBody] string newName)
    {
        await Mediator.Send(new RenameBudgetCommand(BudgetId: id, NewName: newName));
        return NoContent();
    }
}
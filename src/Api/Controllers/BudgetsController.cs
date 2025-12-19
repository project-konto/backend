using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Budgets.Commands.CreateBudget;
using KontoApi.Application.Features.Budgets.Commands.DeleteBudget;
using KontoApi.Application.Features.Budgets.Commands.RenameBudget;
using KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BudgetsController : BaseController
{
    // POST api/budgets
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CreateBudget(
        [FromBody] CreateBudgetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateBudgetCommand(
            request.AccountId,
            request.Name,
            request.InitialBalance,
            request.Currency
        );

        var budgetId = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetBudget), new { id = budgetId }, new { BudgetId = budgetId });
    }

    // GET api/budgets/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(BudgetDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBudget(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetBudgetDetailsQuery(id);
        var result = await Mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    // DELETE api/budgets/{id}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBudget(Guid id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteBudgetCommand(BudgetId: id), cancellationToken);
        return NoContent();
    }

    // PATCH api/budgets/{id}/name
    [HttpPatch("{id:guid}/name")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RenameBudget(
        Guid id,
        [FromBody] RenameBudgetRequest request,
        CancellationToken cancellationToken)
    {
        var command = new RenameBudgetCommand(id, request.NewName);
        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
}

public record RenameBudgetRequest(string NewName);

public record CreateBudgetRequest(
    Guid AccountId,
    string Name,
    decimal InitialBalance,
    string Currency
);
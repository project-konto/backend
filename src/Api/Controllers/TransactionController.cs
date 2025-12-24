using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Budgets.Queries.GetBudgetDetails;
using KontoApi.Application.Features.Transactions.Commands.AddTransaction;
using KontoApi.Application.Features.Transactions.Commands.DeleteTransaction;
using KontoApi.Application.Features.Transactions.Commands.ImportTransactions;
using KontoApi.Application.Features.Transactions.Queries.Common;
using KontoApi.Application.Features.Transactions.Queries.GetTransactionById;
using KontoApi.Application.Features.Transactions.Queries.GetTransactions;
using KontoApi.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : BaseController
{
    // GET api/transactions?budgetId={id}&from={date}&to={date}
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionDetailDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAllTransactions(
        [FromQuery] Guid budgetId,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken cancellationToken)
    {
        if (budgetId == Guid.Empty)
        {
            return BadRequest(new ErrorResponse(
                StatusCodes.Status400BadRequest, "budgetId query parameter is required", null
            ));
        }

        var query = new GetTransactionsQuery(budgetId, from, to);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // POST api/transactions
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddTransaction(
        [FromBody] AddTransactionRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AddTransactionCommand(
            request.BudgetId,
            request.Amount,
            request.Currency,
            request.Type,
            request.CategoryId,
            request.Date,
            request.Description
        );

        var transactionId = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(GetTransaction), new { id = transactionId },
            new { TransactionId = transactionId });
    }

    // GET api/transactions/{id}
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(TransactionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTransaction(Guid id, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetTransactionByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    // DELETE api/transactions/{id}?budgetId={budgetId}
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteTransaction(
        Guid id,
        [FromQuery] Guid budgetId,
        CancellationToken cancellationToken)
    {
        if (budgetId == Guid.Empty)
            return BadRequest(new ErrorResponse(
                StatusCodes.Status400BadRequest, "budgetId query parameter is required", null
            ));

        await Mediator.Send(new DeleteTransactionCommand(BudgetId: budgetId, TransactionId: id), cancellationToken);
        return NoContent();
    }

    // POST api/transactions/import
    [HttpPost("import")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ImportTransactions(
        [FromForm] Guid budgetId,
        IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new ErrorResponse(
                StatusCodes.Status400BadRequest, "No file uploaded", null
            ));

        await using var stream = file.OpenReadStream();

        var command = new ImportTransactionsCommand(budgetId, stream, file.FileName);
        var result = await Mediator.Send(command, cancellationToken);

        return Ok(result);
    }
}

public record AddTransactionRequest(
    Guid BudgetId,
    decimal Amount,
    string Currency,
    TransactionType Type,
    Guid CategoryId,
    DateTime Date,
    string? Description
);
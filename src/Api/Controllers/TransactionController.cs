using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Transactions.Commands.AddTransaction;
using KontoApi.Application.Features.Transactions.Commands.DeleteTransaction;
using KontoApi.Application.Features.Transactions.Commands.ImportTransactions;
using KontoApi.Application.Features.Transactions.Queries.GetTransactionById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TransactionsController : ControllerBase
{
	private readonly IMediator mediator;

	public TransactionsController(IMediator mediator)
		=> this.mediator = mediator;

	// POST api/transactions
	[HttpPost]
	[ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> AddTransaction(AddTransactionCommand command)
	{
		var transactionId = await mediator.Send(command);
		return Ok(new { TransactionId = transactionId });
	}

	// GET api/transactions/{id}
	[HttpGet("{id:guid}")]
	[ProducesResponseType(typeof(TransactionDetailDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> GetTransaction(Guid id)
	{
		var result = await mediator.Send(new GetTransactionByIdQuery(id));
		return Ok(result);
	}

	// DELETE api/transactions/{id}?budgetId={budgetId}
	[HttpDelete("{id:guid}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteTransaction(Guid id, [FromQuery] Guid budgetId)
	{
		if (budgetId == Guid.Empty)
			return BadRequest(new { error = "budgetId query parameter is required" });

		await mediator.Send(new DeleteTransactionCommand(BudgetId: budgetId, TransactionId: id));
		return NoContent();
	}

	// POST api/transactions/import
	[HttpPost("import")]
	[Consumes("multipart/form-data")]
	[ProducesResponseType(typeof(ImportResultDto), StatusCodes.Status200OK)]
	[ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
	public async Task<IActionResult> ImportTransactions(
		[FromForm] Guid budgetId,
		IFormFile? file)
	{
		if (file == null || file.Length == 0)
			return BadRequest(new { error = "No file uploaded" });

		await using var stream = file.OpenReadStream();

		var command = new ImportTransactionsCommand(budgetId, stream, file.FileName);
		var result = await mediator.Send(command);

		return Ok(result);
	}
}
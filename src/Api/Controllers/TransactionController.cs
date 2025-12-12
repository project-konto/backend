using KontoApi.Application.DTOs;
using KontoApi.Application.Handlers;
using KontoApi.Application.Queries;
using KontoApi.Application.Users.Transactions;
using KontoApi.Domain;
using Microsoft.AspNetCore.Mvc;
using GetTransactionsHandler = KontoApi.Application.Handlers.GetTransactionsHandler;

namespace KontoApi.Api.Controllers;

[Route("api/budgets/{budgetId:guid}/transactions")]
public class TransactionController : BaseController
{
    private readonly AddTransactionHandler addHandler;
    private readonly GetTransactionsHandler getHandler;
    private readonly DeleteTransactionHandler deleteHandler;

    public TransactionController(AddTransactionHandler addHandler, GetTransactionsHandler getHandler,
        DeleteTransactionHandler deleteHandler)
    {
        this.addHandler = addHandler;
        this.getHandler = getHandler;
        this.deleteHandler = deleteHandler;
    }

    [HttpPost]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionRequest request, Guid budgetId)
    {
        if (!Enum.TryParse<TransactionType>(request.Type, true, out var transactionType))
            return BadRequest($"Unknown transaction type: {request.Type}");

        var command = new AddTransactionCommand
        {
            BudgetId = budgetId,
            Type = transactionType,
            Amount = (decimal)request.Amount,
            Currency = request.Currency,
            Category = request.Category,
            Date = request.Date,
            Description = request.Description
        };

        var result = await addHandler.Handle(command);
        var response = new TransactionResponse
        {
            Id = result.TransactionId,
            Amount = request.Amount,
            Currency = request.Currency,
            Category = request.Category,
            Date = request.Date,
            Description = request.Description,
            Type = request.Type
        };

        return Ok(response);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> Get([FromQuery] Guid budgetId,
        [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, [FromQuery] string? type,
        [FromQuery] string? category, [FromQuery] double? minAmount, [FromQuery] double? maxAmount)
    {
        TransactionType? typeEnum = null;
        if (!string.IsNullOrWhiteSpace(type))
        {
            if (!Enum.TryParse<TransactionType>(type, true, out var parsedType))
                return BadRequest($"Unknown transaction type: {type}");
            typeEnum = parsedType;
        }

        var query = new GetTransactionsQuery
        {
            BudgetId = budgetId,
            DateRange = DateRange.Create(startDate, endDate),
            Type = typeEnum,
            Category = category,
            MinAmount = minAmount.HasValue ? (decimal)minAmount.Value : null,
            MaxAmount = maxAmount.HasValue ? (decimal)maxAmount.Value : null
        };

        var response = await getHandler.Handle(query);
        return Ok(response);
    }

    [HttpDelete("{transactionId:guid}")]
    public async Task<ActionResult> Delete(Guid budgetId ,Guid transactionId)
    {
        var command = new DeleteTransactionCommand
        {
            TransactionId = transactionId,
            BudgetId = budgetId
            
        };

        var result = await deleteHandler.Handle(command);
        if (!result.IsDeleted)
            return NotFound();

        return NoContent();
    }
}
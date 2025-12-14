using System.Security.Claims;
using KontoApi.Application.Accounts;
using KontoApi.Application.Features.Accounts.Commands.CreateAccount;
using KontoApi.Application.Features.Accounts.Commands.DeleteAccount;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KontoApi.Api.Controllers;


[Authorize]
public class AccountController : BaseController
{
    private readonly IMediator mediator;

    public AccountController(IMediator mediator)
    {
        this.mediator = mediator;
    }

    [HttpPost]
    public async Task<ActionResult> Create(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new CreateAccountCommand(userId);
        var accountId = await mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(Get), new { }, accountId);
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new GetAccountOverviewQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(id);
        await mediator.Send(command, cancellationToken);

        return NoContent();
    }


    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
        {
            //лучше кидать исключение, которое поймает Middleware и вернет 401
            throw new UnauthorizedAccessException("Invalid Token");
        }
        return userId;
    }
}

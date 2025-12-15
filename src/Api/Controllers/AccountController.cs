using System.Security.Claims;
using KontoApi.Application.Features.Accounts.Commands.CreateAccount;
using KontoApi.Application.Features.Accounts.Commands.DeleteAccount;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KontoApi.Api.Controllers;

[Authorize]
public class AccountController : BaseController
{
    [HttpPost]
    public async Task<ActionResult> Create(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var command = new CreateAccountCommand(userId);
        var accountId = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(Get), new { }, accountId);
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        var query = new GetAccountOverviewQuery(userId);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(id);
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }


    private Guid GetCurrentUserId()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        return !Guid.TryParse(userIdString, out var userId)
            ? throw
                new UnauthorizedAccessException("Invalid Token")
            : userId;
    }
}
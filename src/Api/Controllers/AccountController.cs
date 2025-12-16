using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Accounts.Commands.CreateAccount;
using KontoApi.Application.Features.Accounts.Commands.DeleteAccount;
using KontoApi.Application.Features.Accounts.Queries.GetAccountOverview;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KontoApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AccountController : BaseController
{
    // POST api/account
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Create(CancellationToken cancellationToken)
    {
        var command = new CreateAccountCommand(UserId);
        var accountId = await Mediator.Send(command, cancellationToken);

        return CreatedAtAction(nameof(Get), new { }, new { AccountId = accountId });
    }

    // GET api/account
    [HttpGet]
    [ProducesResponseType(typeof(AccountOverviewDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var query = new GetAccountOverviewQuery(UserId);
        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // DELETE api/account
    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(CancellationToken cancellationToken)
    {
        var command = new DeleteAccountCommand(UserId);
        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}
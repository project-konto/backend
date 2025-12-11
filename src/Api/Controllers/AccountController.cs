using System.Security.Claims;
using KontoApi.Application.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace KontoApi.Api.Controllers;


[Authorize]
public class AccountController(CreateAccountHandler createAccountHandler, GetAccountsHandler getHandler,
    DeleteAccountHandler deleteHandler) : BaseController
{
    [HttpPost]
    public async Task<ActionResult> Create(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized("Invalid user ID in token");


        var result = await createAccountHandler.Handle(userId, cancellationToken);
        return CreatedAtAction(nameof(Create), new { id = result.Id }, result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized("Invalid user ID in token");

        await deleteHandler.Handle(id, userId, cancellationToken);

        return NoContent();
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Invalid user ID in token");
        }

        var result = await getHandler.Handle(userId, cancellationToken);

        return Ok(result);
    }
}
using KontoApi.Api.Contracts;
using KontoApi.Application.Features.Users.Commands.ChangePassword;
using KontoApi.Application.Features.Users.Queries.GetUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    // GET api/users/me
    [HttpGet("me")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(UserId);

        var result = await Mediator.Send(query, cancellationToken);

        return Ok(result);
    }

    // PUT api/users/password
    [HttpPut("password")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(
            UserId,
            request.CurrentPassword,
            request.NewPassword
        );

        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
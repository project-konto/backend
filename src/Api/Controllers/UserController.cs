using KontoApi.Application.Features.Users.Commands.ChangePassword;
using KontoApi.Application.Features.Users.Queries.GetUser;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : BaseController
{
    // GET api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await Mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }

    // PUT api/users/{id}/password
    [HttpPut("{id:guid}/password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword);
        await Mediator.Send(command);
        return NoContent();
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
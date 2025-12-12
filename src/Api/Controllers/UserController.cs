using KontoApi.Application.Features.Auth.Commands.Login;
using KontoApi.Application.Features.Auth.Commands.Register;
using KontoApi.Application.Features.Users.Commands.ChangePassword;
using KontoApi.Application.Features.Users.Queries.GetUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator mediator;

    public UsersController(IMediator mediator)
        => this.mediator = mediator;

    // POST api/users/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var userId = await mediator.Send(command);
        return CreatedAtAction(nameof(GetUser), new { id = userId }, userId);
    }

    // POST api/users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }

    // GET api/users/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id)
    {
        var result = await mediator.Send(new GetUserQuery(id));
        return Ok(result);
    }

    // PUT api/users/{id}/password
    [HttpPut("{id}/password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordRequest request)
    {
        var command = new ChangePasswordCommand(id, request.CurrentPassword, request.NewPassword);
        await mediator.Send(command);
        return NoContent();
    }
}

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);
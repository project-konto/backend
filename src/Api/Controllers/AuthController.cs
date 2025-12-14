using KontoApi.Application.Features.Auth.Commands.Login;
using KontoApi.Application.Features.Auth.Commands.Register;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator mediator;

    public AuthController(IMediator mediator)
        => this.mediator = mediator;

    // POST api/auth/register
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterCommand command)
    {
        var userId = await mediator.Send(command);
        return Ok(new { UserId = userId });
    }

    // POST api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var response = await mediator.Send(command);
        return Ok(response);
    }
}

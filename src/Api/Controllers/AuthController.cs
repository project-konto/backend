using KontoApi.Api.Contracts;
using KontoApi.Application.DTOs;
using KontoApi.Application.Handlers;
using KontoApi.Application.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KontoApi.Api.Controllers;

public class AuthController : BaseController
{
    private readonly RegisterUserHandler registerHandler;
    private readonly LoginUserHandler loginHandler;

    public AuthController(RegisterUserHandler registerHandler, LoginUserHandler loginHandler)
    {
        this.registerHandler = registerHandler;
        this.loginHandler = loginHandler;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest registerRequest)
    {
        var command = new RegisterUserCommand(registerRequest.Name, registerRequest.Email, registerRequest.Password);

        var dto = await registerHandler.Handle(command);
        var response = ToAuthResponse(dto);

        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var command = new LoginUserCommand(loginRequest.Email, loginRequest.Password);

        var dto = await loginHandler.Handle(command);
        var response = ToAuthResponse(dto);

        return Ok(response);
    }

    private static AuthResponse ToAuthResponse(AuthUserDto dto) => new()
    {
        UserId = dto.UserId,
        Name = dto.Name,
        Email = dto.Email,
        AccessToken = dto.Token
    };
}
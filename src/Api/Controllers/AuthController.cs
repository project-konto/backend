using KontoApi.Api.Contracts;
using KontoApi.Application.Interfaces;
using KontoApi.Application.Users;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KontoApi.Api.Controllers;

public class AuthController : BaseController
{
    private readonly RegisterUserHandler registerHandler;
    private readonly LoginUserHandler loginHandler;
    private readonly IUserRepository userRepository;
    private readonly ITokenService tokenService;

    public AuthController(RegisterUserHandler registerHandler, LoginUserHandler loginHandler,
        IUserRepository userRepository, ITokenService tokenService)
    {
        this.registerHandler = registerHandler;
        this.loginHandler = loginHandler;
        this.userRepository = userRepository;
        this.tokenService = tokenService;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest registerRequest)
    {
        var command = new RegisterUserCommand
        {
            Name = registerRequest.Name,
            Email = registerRequest.Email,
            Password = registerRequest.Password,
        };
        
        var result = await registerHandler.Handle(command);
        var user = await userRepository.GetByIdAsync(result.UserId);

        if (user is null)
            return StatusCode(500, "User not found after registration");

        var token = tokenService.GenerateAccessToken(user);
        var response = new AuthResponse
        {
            UserId = result.UserId,
            Name = result.Name,
            Email = result.Email,
            AccessToken = token,
        };
        
        return Ok(response);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest loginRequest)
    {
        var command = new LoginUserCommand
        {
            Email = loginRequest.Email,
            Password = loginRequest.Password,
        };
        
        var loginDto = await loginHandler.Handle(command);
        var user = await userRepository.GetByIdAsync(loginDto.UserId);
        
        if (user is null)
            return StatusCode(500, "User not found");
        
        var token = tokenService.GenerateAccessToken(user);
        var response = new AuthResponse
        {
            UserId = loginDto.UserId,
            Name = loginDto.Name,
            Email = loginDto.Email,
            AccessToken = token,
        };
        
        return Ok(response);
    }
}
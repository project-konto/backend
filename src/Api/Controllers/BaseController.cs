using KontoApi.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    private ISender? mediator;
    private ICurrentUserService? currentUserService;

    protected ISender Mediator
        => mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();

    private ICurrentUserService CurrentUser =>
        currentUserService ??= HttpContext.RequestServices.GetRequiredService<ICurrentUserService>();

    protected Guid UserId => CurrentUser.UserId
                             ?? throw new UnauthorizedAccessException("User is not authenticated");
}
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KontoApi.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public abstract class BaseController : ControllerBase
{
    private ISender? mediator;

    protected ISender Mediator
        => mediator ??= HttpContext.RequestServices.GetRequiredService<ISender>();
}
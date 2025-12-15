using System.Security.Claims;
using KontoApi.Application.Common.Interfaces;

namespace KontoApi.Api.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public Guid? UserId
    {
        get
        {
            var idClaim = httpContextAccessor
                .HttpContext?
                .User
                .FindFirst(ClaimTypes.NameIdentifier);

            return idClaim == null
                ? null
                : Guid.Parse(idClaim.Value);
        }
    }
}
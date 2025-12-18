using System.Security.Claims;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KontoApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;

namespace KontoApi.Api.TestAuth;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        try
        {
            // Try to resolve DB and find or create test user by email
            var db = Context.RequestServices.GetService(typeof(KontoDbContext)) as KontoDbContext;
            var testEmail = "testuser@example.com";

            if (db == null)
                return Task.FromResult(AuthenticateResult.NoResult());

            var user = db.Users.FirstOrDefault(u => u.Email == testEmail);

            if (user == null)
            {
                // Create a test user in this request's DB so handlers that rely on User existance succeed
                var hasher = Context.RequestServices.GetService(typeof(IPasswordHasher)) as IPasswordHasher;
                var passwordHash = hasher?.Hash("Test123!") ?? "test-hash";
                user = new User("Test User", testEmail, passwordHash);
                db.Users.Add(user);
                db.SaveChanges();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.Name)
            };

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
        catch
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }
    }
}

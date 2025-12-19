using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KontoApi.Infrastructure.Auth;

/// <summary>
/// Generates and validates JWT access tokens and creates refresh tokens
/// </summary>
/// <remarks>
/// This service reads JWT configuration values from IConfiguration:
/// - "Jwt:Key" (required, store in dotnet user-secrets)
/// - "Jwt:Issuer"
/// - "Jwt:Audience"
/// - "Jwt:ExpirationMinutes"
/// </remarks>
public class JwtProvider : IJwtProvider
{
    /// <summary>
    /// Configuration used to read JWT settings
    /// </summary>
    private readonly JwtSettings settings;

    /// <summary>
    /// Creates a new instance of <see cref="JwtProvider"/>.
    /// </summary>
    /// <param name="settings">Application configuration providing JWT settings</param>
    public JwtProvider(IOptions<JwtSettings> settings)
        => this.settings = settings.Value;

    /// <summary>
    /// Generates a signed JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user for whom the token is generated. The token will include the user's Id, Email and Name claims</param>
    /// <returns>A signed JWT as a string</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration key "Jwt:Key" is not present</exception>
    /// <exception cref="FormatException">May be thrown if "Jwt:ExpirationMinutes" cannot be parsed as a double</exception>
    public string Generate(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            settings.Issuer,
            settings.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(settings.ExpirationMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a cryptographically secure random refresh token encoded as Base64
    /// </summary>
    /// <returns>A Base64-encoded random token</returns>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
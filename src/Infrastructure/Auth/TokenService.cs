using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using Microsoft.Extensions.Configuration;
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
public class TokenService : ITokenService
{
    /// <summary>
    /// Configuration used to read JWT settings
    /// </summary>
    private readonly IConfiguration configuration;

    /// <summary>
    /// Creates a new instance of <see cref="TokenService"/>.
    /// </summary>
    /// <param name="configuration">Application configuration providing JWT settings</param>
    public TokenService(IConfiguration configuration)
        => this.configuration = configuration;

    /// <summary>
    /// Generates a signed JWT access token for the specified user
    /// </summary>
    /// <param name="user">The user for whom the token is generated. The token will include the user's Id, Email and Name claims</param>
    /// <returns>A signed JWT as a string</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration key "Jwt:Key" is not present</exception>
    /// <exception cref="FormatException">May be thrown if "Jwt:ExpirationMinutes" cannot be parsed as a double</exception>
    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                                   ?? throw new InvalidOperationException("JWT Key not configured")));

        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Name)
        };

        var token = new JwtSecurityToken(
            configuration["Jwt:Issuer"],
            configuration["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(
                double.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60")),
            signingCredentials: credentials
        );

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

    /// <summary>
    /// Validates the provided JWT and returns the user Id from the NameIdentifier claim if valid
    /// </summary>
    /// <param name="token">The JWT to validate</param>
    /// <returns>The user's Id parsed as a <see cref="Guid"/> when validation succeeds; otherwise <c>null</c>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the configuration key "Jwt:Key" is not present</exception>
    public Guid? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                                         ?? throw new InvalidOperationException("JWT Key not configured"));

        try
        {
            tokenHandler.ValidateToken(token, new()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out var validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;

            return Guid.Parse(userId);
        }
        catch
        {
            return null;
        }
    }
}
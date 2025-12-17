using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using KontoApi.Domain;
using KontoApi.Infrastructure.Auth;

namespace KontoApi.Tests;

public class JwtProviderTests
{
    private readonly JwtProvider jwtProvider;

    public JwtProviderTests()
    {
        var settings = new JwtSettings
        {
            Key = "super-secret-key-which-is-long-enough-12345",
            Issuer = "TestIssuer",
            Audience = "TestAudience",
            ExpirationMinutes = 60
        };

        jwtProvider = new(Options.Create(settings));
    }

    private static User CreateTestUser(string name = "First Last", string email = "test@example.com")
        => new(name, email, "password1234");

    [Fact]
    public void Generate_ReturnsNonEmptyJwt()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = jwtProvider.Generate(user);

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(token));

        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void Generate_IncludesUserClaims()
    {
        // Arrange
        var user = CreateTestUser("Egor V", "egor@mail.com");

        // Act
        var token = jwtProvider.Generate(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwt = handler.ReadJwtToken(token);

        Assert.Equal(user.Id.ToString(), jwt.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email, jwt.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.Name, jwt.Claims.First(c => c.Type == ClaimTypes.Name).Value);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsValidBase64_64Bytes()
    {
        // Act
        var refresh = jwtProvider.GenerateRefreshToken();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(refresh));

        var bytes = Convert.FromBase64String(refresh);
        Assert.Equal(64, bytes.Length);
    }
}
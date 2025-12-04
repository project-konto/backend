using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using KontoApi.Domain;
using KontoApi.Infrastructure;
using Microsoft.Extensions.Configuration;
using Moq;

namespace KontoApi.Tests;

public class TokenServiceTests
{
    private readonly TokenService tokenService;

    private const string TestSecretKey = "mwhgOXDScUd8EqB7AIEmS6clp8H/bjyRHKvaRz2Zc9k=";
    private const string TestIssuer = "TestIssuer";
    private const string TestAudience = "TestAudience";
    private const string TestExpirationMinutes = "60";

    public TokenServiceTests()
    {
        var configurationMock = new Mock<IConfiguration>();

        configurationMock.Setup(x => x["Jwt:Key"]).Returns(TestSecretKey);
        configurationMock.Setup(x => x["Jwt:Issuer"]).Returns(TestIssuer);
        configurationMock.Setup(x => x["Jwt:Audience"]).Returns(TestAudience);
        configurationMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns(TestExpirationMinutes);

        tokenService = new(configurationMock.Object);
    }

    private static User CreateTestUser(string name = "First Last", string email = "test@example.com")
        => new(name, email, "password1234");

    #region GenerateAccessToken Tests

    [Fact]
    public void GenerateAccessToken_WithValidUser_ReturnsToken()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }

    [Fact]
    public void GenerateAccessToken_ReturnsValidJwtFormat()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert - JWT has 3 parts separated by dots
        var parts = token.Split('.');
        Assert.Equal(3, parts.Length);
    }

    [Fact]
    public void GenerateAccessToken_ContainsCorrectClaims()
    {
        // Arrange
        var user = CreateTestUser("John Doe", "john@example.com");

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(user.Id.ToString(), jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email, jwtToken.Claims.First(c => c.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.Name, jwtToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);
    }

    [Fact]
    public void GenerateAccessToken_SetsCorrectIssuerAndAudience()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        Assert.Equal(TestIssuer, jwtToken.Issuer);
        Assert.Contains(TestAudience, jwtToken.Audiences);
    }

    [Fact]
    public void GenerateAccessToken_SetsCorrectExpiration()
    {
        // Arrange
        var user = CreateTestUser();
        var expectedExpiration = DateTime.UtcNow.AddMinutes(60);

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        // Allow 1 minute tolerance for test execution time
        Assert.True(jwtToken.ValidTo <= expectedExpiration.AddMinutes(1));
        Assert.True(jwtToken.ValidTo >= expectedExpiration.AddMinutes(-1));
    }

    [Fact]
    public void GenerateAccessToken_WithMissingJwtKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["Jwt:Key"]).Returns((string?)null);

        var newTokenService = new TokenService(configMock.Object);
        var user = CreateTestUser();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => newTokenService.GenerateAccessToken(user));
    }

    [Theory]
    [InlineData("user1@example.com", "User One")]
    [InlineData("user2@test.org", "User Two")]
    [InlineData("admin@company.net", "Admin User")]
    public void GenerateAccessToken_WithVariousUsers_ReturnsValidToken(string email, string name)
    {
        // Arrange
        var user = CreateTestUser(name, email);

        // Act
        var token = tokenService.GenerateAccessToken(user);

        // Assert
        Assert.NotNull(token);
        Assert.NotEmpty(token);

        var userId = tokenService.ValidateToken(token);
        Assert.NotNull(userId);
        Assert.Equal(user.Id, userId);
    }

    #endregion

    #region GenerateRefreshToken Tests

    [Fact]
    public void GenerateRefreshToken_ReturnsNonEmptyString()
    {
        // Act
        var refreshToken = tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotNull(refreshToken);
        Assert.NotEmpty(refreshToken);
    }

    [Fact]
    public void GenerateRefreshToken_ReturnsBase64String()
    {
        // Act
        var refreshToken = tokenService.GenerateRefreshToken();

        // Assert - Should be valid Base64
        var bytes = Convert.FromBase64String(refreshToken);
        Assert.Equal(64, bytes.Length);
    }

    [Fact]
    public void GenerateRefreshToken_CalledTwice_ReturnsDifferentTokens()
    {
        // Act
        var token1 = tokenService.GenerateRefreshToken();
        var token2 = tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(token1, token2);
    }

    [Fact]
    public void GenerateRefreshToken_GeneratesUniqueTokens()
    {
        // Act
        var tokens = Enumerable.Range(0, 100)
            .Select(_ => tokenService.GenerateRefreshToken())
            .ToList();

        // Assert - All tokens should be unique
        Assert.Equal(tokens.Count, tokens.Distinct().Count());
    }

    #endregion

    #region ValidateToken Tests

    [Fact]
    public void ValidateToken_WithValidToken_ReturnsUserId()
    {
        // Arrange
        var user = CreateTestUser();
        var token = tokenService.GenerateAccessToken(user);

        // Act
        var userId = tokenService.ValidateToken(token);

        // Assert
        Assert.NotNull(userId);
        Assert.Equal(user.Id, userId);
    }

    [Fact]
    public void ValidateToken_WithInvalidToken_ReturnsNull()
    {
        // Arrange
        var invalidToken = "invalid_token";

        // Act
        var userId = tokenService.ValidateToken(invalidToken);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithEmptyToken_ReturnsNull()
    {
        // Act
        var userId = tokenService.ValidateToken(string.Empty);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithMalformedToken_ReturnsNull()
    {
        // Arrange
        var malformedToken = "not_a_jwt_token";

        // Act
        var userId = tokenService.ValidateToken(malformedToken);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithTokenSignedWithDifferentKey_ReturnsNull()
    {
        // Arrange - Create token service with different key
        var differentConfigMock = new Mock<IConfiguration>();
        differentConfigMock.Setup(x => x["Jwt:Key"]).Returns("different-secret-key-also-32-characters-long!");
        differentConfigMock.Setup(x => x["Jwt:Issuer"]).Returns(TestIssuer);
        differentConfigMock.Setup(x => x["Jwt:Audience"]).Returns(TestAudience);
        differentConfigMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns(TestExpirationMinutes);

        var differentTokenService = new TokenService(differentConfigMock.Object);
        var user = CreateTestUser();
        var token = differentTokenService.GenerateAccessToken(user);

        // Act - Validate with original service (different key)
        var userId = tokenService.ValidateToken(token);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithWrongIssuer_ReturnsNull()
    {
        // Arrange
        var wrongIssuerConfigMock = new Mock<IConfiguration>();
        wrongIssuerConfigMock.Setup(x => x["Jwt:Key"]).Returns(TestSecretKey);
        wrongIssuerConfigMock.Setup(x => x["Jwt:Issuer"]).Returns("wrong-issuer");
        wrongIssuerConfigMock.Setup(x => x["Jwt:Audience"]).Returns(TestAudience);
        wrongIssuerConfigMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns(TestExpirationMinutes);

        var wrongIssuerService = new TokenService(wrongIssuerConfigMock.Object);
        var user = CreateTestUser();
        var token = wrongIssuerService.GenerateAccessToken(user);

        // Act
        var userId = tokenService.ValidateToken(token);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithWrongAudience_ReturnsNull()
    {
        // Arrange
        var wrongAudienceConfigMock = new Mock<IConfiguration>();
        wrongAudienceConfigMock.Setup(x => x["Jwt:Key"]).Returns(TestSecretKey);
        wrongAudienceConfigMock.Setup(x => x["Jwt:Issuer"]).Returns(TestIssuer);
        wrongAudienceConfigMock.Setup(x => x["Jwt:Audience"]).Returns("wrong-audience");
        wrongAudienceConfigMock.Setup(x => x["Jwt:ExpirationMinutes"]).Returns(TestExpirationMinutes);

        var wrongAudienceService = new TokenService(wrongAudienceConfigMock.Object);
        var user = CreateTestUser();
        var token = wrongAudienceService.GenerateAccessToken(user);

        // Act
        var userId = tokenService.ValidateToken(token);

        // Assert
        Assert.Null(userId);
    }

    [Fact]
    public void ValidateToken_WithMissingJwtKey_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = CreateTestUser();
        var token = tokenService.GenerateAccessToken(user);

        var noKeyConfigMock = new Mock<IConfiguration>();
        noKeyConfigMock.Setup(x => x["Jwt:Key"]).Returns((string?)null);

        var noKeyTokenService = new TokenService(noKeyConfigMock.Object);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => noKeyTokenService.ValidateToken(token));
    }

    #endregion

    #region Integration Tests

    [Fact]
    public void GenerateAndValidate_RoundTrip_Succeeds()
    {
        // Arrange
        var user = CreateTestUser("Integration Test User", "integration@test.com");

        // Act
        var accessToken = tokenService.GenerateAccessToken(user);
        var validatedUserId = tokenService.ValidateToken(accessToken);

        // Assert
        Assert.NotNull(validatedUserId);
        Assert.Equal(user.Id, validatedUserId);
    }

    [Fact]
    public void RefreshToken_IsDifferentFromAccessToken()
    {
        // Arrange
        var user = CreateTestUser();

        // Act
        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        // Assert
        Assert.NotEqual(accessToken, refreshToken);
    }

    #endregion
}
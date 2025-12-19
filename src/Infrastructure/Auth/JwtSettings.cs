namespace KontoApi.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; init; } = "paste-your-ultra-super-secret-key-here";
    public string Issuer { get; init; } = "DefaultIssuer";
    public string Audience { get; init; } = "DefaultAudience";
    public int ExpirationMinutes { get; init; } = 60;
}
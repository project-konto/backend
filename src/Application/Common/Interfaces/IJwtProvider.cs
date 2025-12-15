using KontoApi.Domain;

namespace KontoApi.Application.Common.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
    string GenerateRefreshToken();
}
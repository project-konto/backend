using KontoApi.Application.Interfaces;

namespace KontoApi.Infrastructure;

public class PasswordHasher : IPasswordHasher
{
    public String Hash(String password)
    {
        throw new NotImplementedException();
    }

    public Boolean Verify(String password, String hash)
    {
        throw new NotImplementedException();
    }
}
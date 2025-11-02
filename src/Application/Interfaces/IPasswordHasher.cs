namespace KontoApi.Application.Interfaces;

public interface IPasswordHasher
{
    string Hash(string email, string password);
    bool Verify(string email, string password, string hashedPassword); // check if hash matches with password
}
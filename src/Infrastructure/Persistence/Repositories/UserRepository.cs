using KontoApi.Application.Common.Interfaces;
using KontoApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure.Persistence.Repositories;

public class UserRepository(KontoDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user, CancellationToken ct)
    {
        await dbContext.Users.AddAsync(user, ct);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
        => await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        => await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var userStub = await dbContext.Users.FindAsync([id], ct);

        if (userStub != null)
        {
            dbContext.Users.Remove(userStub);
            await dbContext.SaveChangesAsync(ct);
        }
    }
}
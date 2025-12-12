using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly KontoDbContext context;

    public UserRepository(KontoDbContext context)
        => this.context = context;

    public async Task AddAsync(User user, CancellationToken ct)
    {
        await context.Users.AddAsync(user, ct);
        await context.SaveChangesAsync(ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return await context.Users
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }

    public async Task UpdateAsync(User user, CancellationToken ct)
    {
        context.Users.Update(user);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var userStub = await context.Users.FindAsync([id], ct);

        if (userStub != null)
        {
            context.Users.Remove(userStub);
            await context.SaveChangesAsync(ct);
        }
    }
}
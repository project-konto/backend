using KontoApi.Application.Interfaces;
using KontoApi.Domain;
using KontoApi.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace KontoApi.Infrastructure;

public class AccountRepository(KontoDbContext context) : IAccountRepository
{
    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
    {
        var entity = new AccountEntity { Id = account.Id, CreatedAt = account.CreatedAt, UserId = account.User.Id };
        await context.AddAsync(entity, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Account account, CancellationToken cancellationToken = default)
    {
        var entity = await context.Account.FindAsync(account.Id);

        if (entity != null)
        {
            // Сейчас обновлять особо нечего, кроме UpdatedAt, если оно есть, на будущее
            await context.SaveChangesAsync(cancellationToken);
        }

    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Account.FindAsync([id], cancellationToken);

        if (entity != null)
        {
            context.Account.Remove(entity);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await context.Account.Include(a => a.UserEntity)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        return entity == null ? null : MapToDomain(entity);
    }

    public async Task<IEnumerable<Account>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var entities = await context.Account.Include(a => a.UserEntity)
            .Where(a => a.UserId == userId).ToListAsync(cancellationToken);

        return entities.Select(MapToDomain);
    }

    private static Account MapToDomain(AccountEntity entity)
    {
        if (entity.UserEntity == null)
            throw new InvalidOperationException($"Account {entity.Id} has no associated User entity");


        var user = new User(entity.UserEntity.Name, entity.UserEntity.Email, entity.UserEntity.HashedPassword);
        SetPrivateId(user, entity.UserId);
        var account = new Account(user);
        SetPrivateId(account, entity.Id);
        typeof(Account).GetProperty(nameof(Account.CreatedAt))?.SetValue(account, entity.CreatedAt);

        return account;
    }

    private static void SetPrivateId<T>(T obj, Guid id)
    {
        var prop = typeof(T).GetProperty("Id");
        if (prop != null)
            prop.SetValue(obj, id);
    }
}
namespace KontoApi.Application.Common.Interfaces;

public interface ICurrentUserService
{
    public Guid? UserId { get; }
}
namespace KontoApi.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(Type entityType, object id)
        : base($"Entity \"{entityType.Name}\" ({id}) was not found.") { }
}
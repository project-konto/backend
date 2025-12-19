using KontoApi.Application.Common.Models;

namespace KontoApi.Application.Common.Interfaces;

public interface IStatementParser
{
    bool Supports(string fileName);
    Task<List<ParsedTransaction>> ParseAsync(Stream fileStream, CancellationToken cancellationToken);
}

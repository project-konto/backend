using KontoApi.Application.DTOs;

namespace KontoApi.Application.Interfaces;

public interface IStatementParser
{
    IEnumerable<ParsedOperation> Parse(Stream fileStream);
}

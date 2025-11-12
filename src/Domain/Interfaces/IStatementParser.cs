using KontoApi.Application.DTOs;

namespace KontoApi.Application.Interfaces;

public interface IStatementParser
{
    IEnumerable<ParsedTransaction> Parse(Stream fileStream);
}

using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;

namespace KontoApi.Infrastructure;

public class StatementParser : IStatementParser
{
    public IEnumerable<ParsedTransaction> Parse(Stream fileStream)
    {
        throw new NotImplementedException();
    }
}
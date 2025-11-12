using KontoApi.Application.Interfaces;

namespace KontoApi.Infrastructure;

public class StatementParser : IStatementParser
{
    public IEnumerable<ParsedOperation> Parse(Stream fileStream)
    {
        throw new NotImplementedException();
    }
}
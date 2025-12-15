using KontoApi.Application.Common.Interfaces;
using KontoApi.Application.Common.Models;

namespace KontoApi.Infrastructure.Services;

public class StatementParser : IStatementParser
{
    public bool Supports(string fileName)
        => fileName.EndsWith(".csv") || fileName.EndsWith(".pdf");

    public List<ParsedTransaction> Parse(Stream fileStream)
        => throw new NotImplementedException();
}

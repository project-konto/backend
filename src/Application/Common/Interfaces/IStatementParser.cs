using KontoApi.Application.Common.Models;

namespace KontoApi.Application.Common.Interfaces;

public interface IStatementParser
{
    bool Supports(string fileName);
    List<ParsedTransaction> Parse(Stream fileStream);
}

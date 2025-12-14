using KontoApi.Application.Common.Models;
using KontoApi.Application.DTOs;
using KontoApi.Application.Interfaces;

namespace KontoApi.Infrastructure.Services;

public class StatementParser : IStatementParser
{
    public bool Supports(string fileName)
        => fileName.EndsWith(".csv") || fileName.EndsWith(".pdf");

    public List<ParsedTransaction> Parse(Stream fileStream)
        => throw new NotImplementedException();
}

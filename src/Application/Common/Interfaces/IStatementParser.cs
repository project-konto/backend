using KontoApi.Application.Common.Models;
using KontoApi.Application.DTOs;

namespace KontoApi.Application.Interfaces;

public interface IStatementParser
{
    bool Supports(string fileName);
    List<ParsedTransaction> Parse(Stream fileStream);
}
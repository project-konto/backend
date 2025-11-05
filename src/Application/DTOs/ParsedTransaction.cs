using KontoApi.Domain;

namespace KontoApi.Application.DTOs;

public class ParsedTransaction
{
    public DateTime Date { get; init; }
    public decimal Amount { get; init; }
    public string Currency { get; init; } = "";
    public string? Description { get; init; }
    public string? ExternalId { get; init; }
    public Category? Category { get; init; }
}
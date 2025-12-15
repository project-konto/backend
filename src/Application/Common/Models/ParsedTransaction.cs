using KontoApi.Domain;

namespace KontoApi.Application.Common.Models;

public record ParsedTransaction(
    DateTime Date,
    decimal Amount,
    string Currency,
    TransactionType Type,
    string? Description,
    Guid CategoryId
);
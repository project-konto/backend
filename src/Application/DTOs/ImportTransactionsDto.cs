namespace KontoApi.Application.DTOs;

public class ImportTransactionsDto
{
    public int Total { get; init; }
    public int Imported { get; init; }
    public int SkippedDuplicates { get; init; }
    public List<ImportErrorDto> Errors { get; init; } = [];
}
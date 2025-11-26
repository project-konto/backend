namespace KontoApi.Application.DTOs;

public class ImportErrorDto
{
	public int LineNumber { get; init; }
	public string Reason { get; init; } = "";
	public string RawLine { get; init; } = "";
}
namespace KontoApi.Api.Contracts;

public class ErrorResponse(int statusCode, string message, string? details)
{
    public int StatusCode { get; private set; } = statusCode;
    public string Message { get; private set; } = message;
    public string? Details { get; private set; } = details;
}
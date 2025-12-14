namespace KontoApi.Api.Contracts;

public class ErrorResponse
{
    public int StatusCode { get; private set; }
    public string Message { get; private set; }
    public string? Details { get; private set; }

    public ErrorResponse(int statusCode, string message, string? details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}

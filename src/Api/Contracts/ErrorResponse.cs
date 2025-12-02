namespace KontoApi.Api.Contracts;

public class ErrorResponse
{
    private int StatusCode { get; set; }
    private string Message { get; set; }
    private string? Details { get; set; }

    public ErrorResponse(int statusCode, string message, string? details)
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}
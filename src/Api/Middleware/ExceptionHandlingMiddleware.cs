using System.Net;
using System.Text.Json;
using KontoApi.Api.Contracts;
using Serilog;
using KontoApi.Application.Exceptions;

namespace KontoApi.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {

        var (statusCode, title) = exception switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "Resource Not Found"),
            BadRequestException => (HttpStatusCode.BadRequest, "Bad Request"),
            ValidationException => (HttpStatusCode.BadRequest, "Bad Request"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            Log.Error(exception, "Произошла необработанная ошибка");

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse(
            context.Response.StatusCode,
            title,
            exception.Message
        );

        var json = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(json);
    }
}
using System.Net;
using System.Text.Json;
using KontoApi.Api.Contracts;
using KontoApi.Application.Common.Exceptions;
using Serilog;
using ValidationException = FluentValidation.ValidationException;
using AppValidationException = KontoApi.Application.Common.Exceptions.ValidationException;

namespace KontoApi.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next)
{
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
            BadRequestException or AppValidationException or ValidationException => (HttpStatusCode.BadRequest, "Bad Request"),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Unauthorized"),
            ForbiddenException => (HttpStatusCode.Forbidden, "Forbidden"),
            ConflictException => (HttpStatusCode.Conflict, "Conflict"),
            _ => (HttpStatusCode.InternalServerError, "Internal Server Error")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
        {
            Log.Error(exception, "Unhandled exception occurred");
            try
            {
                System.IO.File.AppendAllText("/tmp/konto_app_exceptions.log", DateTime.UtcNow + " - " + exception + "\n\n");
            }
            catch { }
        }

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
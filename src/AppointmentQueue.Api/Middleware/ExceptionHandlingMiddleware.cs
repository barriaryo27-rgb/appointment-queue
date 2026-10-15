using Microsoft.AspNetCore.Mvc;

namespace AppointmentQueue.Api.Middleware;

/// <summary>
/// Turns exceptions into clean JSON errors and writes them to the log.
/// Business rule problems become 400 / 409; anything unexpected becomes 500
/// without leaking internal details to the caller.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning("Invalid request: {Message}", ex.Message);
            await WriteError(context, StatusCodes.Status400BadRequest, "Invalid request", ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Request conflict: {Message}", ex.Message);
            await WriteError(context, StatusCodes.Status409Conflict, "Request conflict", ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error on {Method} {Path}", context.Request.Method, context.Request.Path);
            await WriteError(context, StatusCodes.Status500InternalServerError, "Server error",
                "An unexpected error occurred.");
        }
    }

    private static Task WriteError(HttpContext context, int status, string title, string detail)
    {
        context.Response.StatusCode = status;
        return context.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = detail
        });
    }
}

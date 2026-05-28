using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace OnboardingSIGDB1.API.Middleware;

/// <summary>
/// Global middleware that catches unhandled infrastructure exceptions and returns
/// a safe, structured JSON error response — keeping the format consistent with
/// the application's notification pattern.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (DbUpdateConcurrencyException ex)
        {
            _logger.LogError(ex, "Concurrency conflict detected while saving changes.");
            await WriteErrorResponseAsync(context, HttpStatusCode.Conflict,
                "Concurrency", "A concurrency conflict occurred. Please refresh your data and try again.");
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Database error while saving changes.");
            await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError,
                "Database", "An error occurred while saving data. Please try again later.");
        }
        catch (OperationCanceledException ex) when (!context.RequestAborted.IsCancellationRequested)
        {
            // Timeout from the database or an internal operation — not a client disconnect.
            _logger.LogError(ex, "An internal operation timed out.");
            await WriteErrorResponseAsync(context, HttpStatusCode.GatewayTimeout,
                "Timeout", "The operation timed out. Please try again later.");
        }
        catch (OperationCanceledException)
        {
            // Client disconnected intentionally — no response needed, suppress the exception.
            _logger.LogWarning("Request was cancelled by the client.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unexpected error occurred.");
            await WriteErrorResponseAsync(context, HttpStatusCode.InternalServerError,
                "Server", "An unexpected error occurred. Please try again later.");
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, HttpStatusCode statusCode, string key, string message)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var body = JsonSerializer.Serialize(new[]
        {
            new { Key = key, Message = message }
        });

        await context.Response.WriteAsync(body);
    }
}

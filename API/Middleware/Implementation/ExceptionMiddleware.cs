using UserProfileBackend.Application.Models.Exception;
using Microsoft.AspNetCore.Diagnostics;
using UserProfileBackend.Application.Helpers;
using System.Text.Json;

namespace UserProfileBackend.Api.Middleware.Implementation;

/// <summary>
/// Middleware to handle exceptions globally, log them, and return a standardized error response.
/// </summary>
public class ExceptionMiddleware : IExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="IExceptionMiddleware"/> class.
    /// </summary>
    /// <param name="next">The next middleware in the pipeline.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="env">The hosting environment instance.</param>
    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<IExceptionMiddleware> logger,
        IHostEnvironment env
        )
    {
        _next = next ?? throw new ArgumentNullException(nameof(next));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _env = env ?? throw new ArgumentNullException(nameof(env));
    }

    /// <summary>
    /// Invokes the middleware to handle the request and catch exceptions.
    /// </summary>
    /// <param name="context">The HTTP context for the request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        // Get this exception details from context.
        var exceptionDetails = context.Features.Get<IExceptionHandlerFeature>();

        // Helper.PrintObject(context.Features);

        // Do nothing if there isn't exception details.
        if (exceptionDetails == null)
        {
            return;
        }

        // Get the exception from details
        var exception = exceptionDetails?.Error;

        // Handle the exception based on its type
        switch (exception)
        {
            case KeyNotFoundException ex:
                _logger.LogError(ex, "An argument null error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(ex.Message, "Argument null error")));
                break;
            case InvalidOperationException ex:
                _logger.LogError(ex, "An argument null error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(ex.Message, "Argument null error")));
                break;
            case ArgumentNullException ex:
                _logger.LogError(ex, "An argument null error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(ex.Message, "Argument null error")));
                break;
            case HttpRequestException ex:
                _logger.LogError(ex, "An HTTP request error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = ex.StatusCode == null ? StatusCodes.Status500InternalServerError : (int)ex.StatusCode;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(ex.Message, "HTTP request error")));
                break;
            case JsonException ex:
                _logger.LogError(ex, "A JSON serialization error occurred. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(ex.Message, "JSON serialization error")));
                break;
            default:
                _logger.LogError(exception, "An unhandled exception occurred while processing the request. Request Path: {Path}", context.Request.Path);
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(Helper.StringifyObject(new ErrorResponse(exception?.Message ?? "An unhandled exception occurred.", "Inactive object")));
                break;
        }
        return;
    }
}

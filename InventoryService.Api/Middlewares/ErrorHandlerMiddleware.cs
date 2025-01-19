using Microsoft.EntityFrameworkCore;

namespace InventoryService.Api.Middlewares;
/// <summary>
/// Middleware to handle exceptions and log errors.
/// </summary>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="logger">The logger to log errors.</param>
public class ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "A database update error occurred.");
            await HandleExceptionAsync(context, ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception? ex = null)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var message = new
        {
            Message = ex?.Message ?? "An error occurred while processing your request."
        };

        if (ex is DbUpdateException)
        {
            message = new
            {
                Message = "An error occurred while updating entity."
            };
        }

        await context.Response.WriteAsJsonAsync(message);
    }
}

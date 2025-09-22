using System.Net;
using System.Text.Json;

namespace NotesBackend.Api.Middleware
{
    /// <summary>
    /// Global exception handling middleware with friendly Ocean-themed messages.
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

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ArgumentException ex)
            {
                await WriteErrorAsync(context, HttpStatusCode.BadRequest, $"Ocean: {ex.Message}");
            }
            catch (UnauthorizedAccessException)
            {
                await WriteErrorAsync(context, HttpStatusCode.Unauthorized, "Ocean: Unauthorized access.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocean: Unhandled error.");
                await WriteErrorAsync(context, HttpStatusCode.InternalServerError, "Ocean: Something went wrong. We're on it.");
            }
        }

        private static async Task WriteErrorAsync(HttpContext ctx, HttpStatusCode code, string message)
        {
            ctx.Response.StatusCode = (int)code;
            ctx.Response.ContentType = "application/json";
            var payload = JsonSerializer.Serialize(new { message });
            await ctx.Response.WriteAsync(payload);
        }
    }
}

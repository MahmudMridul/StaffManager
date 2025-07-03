using Microsoft.AspNetCore.Diagnostics;
using RBAC_API.Models;
using System.Net;
using System.Text.Json;

namespace RBAC_API.ExceptionHandler
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);

            RbacResponse response = exception switch
            {
                ArgumentException argEx => RbacResponse.BadRequest(argEx.Message),
                KeyNotFoundException notFoundEx => RbacResponse.Create(
                    HttpStatusCode.NotFound,
                    notFoundEx.Message),
                UnauthorizedAccessException => RbacResponse.Create(
                    HttpStatusCode.Unauthorized,
                    "Access denied"),
                InvalidOperationException invalidEx => RbacResponse.BadRequest(invalidEx.Message),
                _ => RbacResponse.Create(
                    HttpStatusCode.InternalServerError,
                    "An unexpected error occurred",
                    errors: new List<string> { exception.Message })
            };

            httpContext.Response.StatusCode = (int)response.StatusCode;
            httpContext.Response.ContentType = "application/json";

            var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await httpContext.Response.WriteAsync(jsonResponse, cancellationToken);

            return true;
        }
    }
}

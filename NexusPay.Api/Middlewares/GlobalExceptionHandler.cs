using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using NexusPay.Shared.Models.Error;

namespace NexusPay.Api.Middlewares
{
    /// <summary>
    /// Handles all unhandled exceptions globally and returns a standardized error response.
    /// </summary>
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="GlobalExceptionHandler"/>.
        /// </summary>
        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Tries to handle the exception and writes a JSON error response.
        /// </summary>
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                RpcException ex when ex.StatusCode == StatusCode.InvalidArgument => (StatusCodes.Status400BadRequest, ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.NotFound => (StatusCodes.Status404NotFound, ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.Unauthenticated => (StatusCodes.Status401Unauthorized, ex.Status.Detail ?? "Invalid credentials."),

                RpcException ex when ex.StatusCode == StatusCode.PermissionDenied => (StatusCodes.Status403Forbidden, ex.Status.Detail ?? "Access denied."),

                RpcException ex when ex.StatusCode == StatusCode.AlreadyExists => (StatusCodes.Status409Conflict, ex.Status.Detail ?? ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.Unavailable => (StatusCodes.Status503ServiceUnavailable, ex.Status.Detail ?? "Service temporarily unavailable."),

                RpcException => (StatusCodes.Status500InternalServerError, "An internal gRPC error occurred."),

                _ => (StatusCodes.Status500InternalServerError, exception.Message ?? "An unexpected error occurred.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Status = statusCode,
                Error = ReasonPhrases.GetReasonPhrase(statusCode),
                Messages = [message],
                Path = context.Request.Path.Value!
            }, cancellationToken);

            return true;
        }
    }
}

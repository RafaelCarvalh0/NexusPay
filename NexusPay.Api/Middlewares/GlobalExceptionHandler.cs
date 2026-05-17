using Grpc.Core;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.WebUtilities;
using NexusPay.Api.Models;

namespace NexusPay.Api.Middlewares
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
        {
            var (statusCode, message) = exception switch
            {
                RpcException ex when ex.StatusCode == StatusCode.InvalidArgument => (StatusCodes.Status400BadRequest, ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.NotFound => (StatusCodes.Status404NotFound, ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.Unauthenticated => (StatusCodes.Status401Unauthorized, "Invalid credentials."),

                RpcException ex when ex.StatusCode == StatusCode.PermissionDenied => (StatusCodes.Status403Forbidden, "Access denied."),

                RpcException ex when ex.StatusCode == StatusCode.AlreadyExists => (StatusCodes.Status409Conflict, ex.Status.Detail),

                RpcException ex when ex.StatusCode == StatusCode.Unavailable => (StatusCodes.Status503ServiceUnavailable, "Service temporarily unavailable."),

                RpcException => (StatusCodes.Status500InternalServerError, "An internal gRPC error occurred."),

                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            if (statusCode == StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new ErrorResponse
            {
                Status = statusCode,
                Error = ReasonPhrases.GetReasonPhrase(statusCode),
                Message = message,
                Path = context.Request.Path.Value!
            }, cancellationToken);

            return true;
        }
    }
}

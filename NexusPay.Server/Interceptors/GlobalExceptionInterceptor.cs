using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Data.SqlClient;

namespace NexusPay.Server.Interceptors
{
    public class GlobalExceptionInterceptor : Interceptor
    {
        private readonly ILogger<GlobalExceptionInterceptor> _logger;

        public GlobalExceptionInterceptor(ILogger<GlobalExceptionInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                return await continuation(request, context);
            }
            catch (RpcException ex)
            {
                _logger.LogError(ex, "gRPC method {Method} threw an RpcException", context.Method);

                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "gRPC method {Method} threw a SqlException", context.Method);

                throw ex.Number switch
                {
                    99999 => new RpcException(new Status(StatusCode.AlreadyExists, ex.Message)),
                    99998 => new RpcException(new Status(StatusCode.NotFound, ex.Message)),
                    _ => new RpcException(new Status(StatusCode.Internal, "Database operation failed."))
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in gRPC method {Method}", context.Method);

                throw new RpcException(new Status(
                    StatusCode.Internal,
                    "An unexpected error occurred."));
            }
        }
    }
}

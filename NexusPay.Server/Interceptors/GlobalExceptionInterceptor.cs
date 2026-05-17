using Grpc.Core;
using Grpc.Core.Interceptors;

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
            catch (RpcException)
            {
                throw;
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

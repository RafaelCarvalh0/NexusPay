using Grpc.Core;
using NexusPay.Contracts;
using NexusPay.Data.Repositories;

namespace NexusPay.Server.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IAuthRepository _authRepository;

        public AuthGrpcService(ILogger<AuthGrpcService> logger, IAuthRepository authRepository)
        {
            _logger = logger;
            _authRepository = authRepository;
        }

        public override async Task<LoginGrpcResponse> Login(LoginGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received authentication request for user: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

            return await _authRepository.Login(request);
        }

        public override async Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation(
                "Logout recebido para token: {Token}", request.Token);

            // request.Token → string definida no .proto campo 1

            // Aqui vai sua lógica de invalidar o token

            return new LogoutGrpcResponse
            {
                Message = "Logout realizado com sucesso"
            };
        }
    }
}

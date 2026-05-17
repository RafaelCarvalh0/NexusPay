using Grpc.Core;
using NexusPay.Contracts;

namespace NexusPay.Server.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthGrpcService> _logger;
        
        public AuthGrpcService(ILogger<AuthGrpcService> logger)
        {
            _logger = logger;
        }

        public override Task<LoginGrpcResponse> Login(LoginGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received authentication request for user: {Username}", request.Username);

            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Username and password are required"));
            }

            var response = new LoginGrpcResponse
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ODc2NTQzMjEiLCJuYW1lIjoiUmFmYWVsIENhcnZhbGhvIiwicm9sZSI6IkFkbWluIiwiaWF0IjoxNzQ3MzY4MDAwLCJleHAiOjE3NDczNzE2MDB9.X8mV9Kp2WfQ7LnD4sYt3AzrQvHnE5BcJuR2xZaKf91A",
                Message = "Authentication successful"
            };
            return Task.FromResult(response);
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

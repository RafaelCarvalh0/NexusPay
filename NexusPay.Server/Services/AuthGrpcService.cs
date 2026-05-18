using Grpc.Core;
using NexusPay.Contracts;
using NexusPay.Data.Repositories;
using NexusPay.Server.Helper;
using NexusPay.Shared.Models.Auth.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Server.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;

        public AuthGrpcService(ILogger<AuthGrpcService> logger, IAuthRepository authRepository, IJwtService jwtService)
        {
            _logger = logger;
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        public override async Task<LoginGrpcResponse> Login(LoginGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received authentication request for user: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

            AuthClaims user = await _authRepository.Login(request);
            JwtSecurityToken jwt = _jwtService.GenerateToken(user);

            return new LoginGrpcResponse
            {
                Token = _jwtService.WriteToken(jwt),
                TokenType = "Bearer",
                ExpiresIn = (int)(jwt.ValidTo - DateTime.UtcNow).TotalSeconds,
                UserId = user.Id.ToString(),
                UserName = user.Name,
                Role = user.Role
            };
        }

        public override async Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received logout request for user: {Id}", request.Id);

            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User ID is required"));

            await _authRepository.Logout(request);

            return new LogoutGrpcResponse
            {
                Message = "Logout successful"
            };
        }
    }
}

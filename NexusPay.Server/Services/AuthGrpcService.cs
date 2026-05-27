using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NexusPay.Contracts;
using NexusPay.Data.Repositories;
using NexusPay.Server.Helper.Jwt;
using NexusPay.Server.Helper.Mail;
using NexusPay.Server.Helper.Redis;
using NexusPay.Shared.Models.Auth;
using NexusPay.Shared.Models.Auth.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Server.Services
{
    public class AuthGrpcService : AuthService.AuthServiceBase
    {
        private readonly ILogger<AuthGrpcService> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IJwtService _jwtService;
        private readonly IRedisService _redisService;
        private readonly IMailService _mailService;
        private readonly string _frontEndUrl;

        public AuthGrpcService(ILogger<AuthGrpcService> logger, IAuthRepository authRepository, IJwtService jwtService, IRedisService redisService, IMailService mailService, IConfiguration configuration)
        {
            _logger = logger;
            _authRepository = authRepository;
            _jwtService = jwtService;
            _redisService = redisService;
            _mailService = mailService;
            _frontEndUrl = configuration.GetSection("FrontendUrl").Value ?? throw new InvalidOperationException("Frontend URL not found.");
        }

        public override async Task<LoginGrpcResponse> Login(LoginGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received authentication request for user: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

            AuthClaims user = await _authRepository.Login(new LoginRequest
            (
                Email: request.Email,
                Password: request.Password
            ));

            // Check for an existing active session and revoke it if found
            string? activeJti = await _redisService.GetActiveSessionAsync(user.Id.ToString());

            if (activeJti is not null)
                await _redisService.RevokeTokenAsync(activeJti, TimeSpan.FromHours(1));

            JwtSecurityToken jwt = _jwtService.GenerateToken(user);

            // Store the active session in Redis with a TTL matching the token's expiration
            await _redisService.SaveActiveSessionAsync(user.Id.ToString(), jwt.Id, TimeSpan.FromHours(1));

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

        public override async Task<Empty> ForgotPassword(ForgotPasswordGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received forgot password request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            string path = $"{_frontEndUrl}/reset-password.html";
            string resetToken = Guid.NewGuid().ToString("N");
            string resetLink = $"{path}?email={Uri.EscapeDataString(request.Email)}&token={resetToken}";

            string redisKey = $"reset_password:{resetToken}";
            await _redisService.SetStringAsync(redisKey, request.Email, TimeSpan.FromMinutes(1));

            var html = MailTemplateFactory.BuildForgotPasswordTemplate(resetLink);

            await _mailService.SendEmailAsync(request.Email, "Redefinição de Senha - NexusPay", html);

            return new Empty();
        }

        public override async Task<Empty> ResetPassword(ResetPasswordGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received reset password request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Token))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Token is required"));

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "New password is required"));

            string redisKey = $"reset_password:{request.Token}";
            string? storedEmail = await _redisService.GetStringAsync(redisKey);

            if (storedEmail is null || !string.Equals(storedEmail, request.Email, StringComparison.OrdinalIgnoreCase))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid or expired token."));

            await _authRepository.ResetPassword(new ResetPasswordRequest
            (
                Email: request.Email,
                Token: string.Empty,
                NewPassword: request.NewPassword
            ));

            await _redisService.DeleteKeyAsync(redisKey);

            return new Empty();
        }

        public override async Task<Empty> Logout(LogoutGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received logout request for user: {Id}", request.UserId);

            if (string.IsNullOrWhiteSpace(request.Jti) || string.IsNullOrWhiteSpace(request.UserId))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid session data."));

            // 1. Revoke the token by adding its JTI to the blacklist
            await _redisService.RevokeTokenAsync(request.Jti, TimeSpan.FromHours(1));

            // 2. Remove the active session for the user
            await _redisService.RemoveActiveSessionAsync(request.UserId);

            return new Empty();
        }

        public override async Task<IsTokenRevokedGrpcResponse> IsTokenRevoked(IsTokenRevokedGrpcRequest request, ServerCallContext context)
        {
            bool isRevoked = await _redisService.IsTokenRevokedAsync(request.Jti);

            return new IsTokenRevokedGrpcResponse { IsRevoked = isRevoked };
        }
    }
}

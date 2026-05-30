using NexusPay.Client.Services;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Shared.Models.Error;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Api.Middlewares
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IAuthGrpcClient authClient)
        {
            // Só verifica rotas autenticadas
            if (context.User.Identity?.IsAuthenticated == true)
            {
                string? jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrWhiteSpace(jti))
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }

                bool isRevoked = await authClient.IsTokenRevokedAsync(jti);

                if (isRevoked)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new ErrorResponse
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Error = "Unauthorized",
                        Messages = ["Token has been revoked. Please login again."],
                        Path = context.Request.Path.Value!
                    });
                    return;
                }
            }

            await _next(context);
        }
    }
}

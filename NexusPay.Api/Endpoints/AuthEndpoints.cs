using Microsoft.AspNetCore.Mvc;
using NexusPay.Api.Extensions;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.Auth;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            app.MapGroup("auth").MapAuthGroup().WithTags("Auth");

            return app;
        }

        private static RouteGroupBuilder MapAuthGroup(this RouteGroupBuilder route)
        {
            route.MapPost("Login", async ([FromBody] LoginRequest request, [FromServices] IAuthGrpcClient authClient) =>
            {
                LoginResponse response = await authClient.LoginAsync(request);
                return Results.Ok(response);

            }).AllowAnonymous().WithValidation<LoginRequest>();

            route.MapPost("ForgotPassword", async ([FromBody] ForgotPasswordRequest request, [FromServices] IAuthGrpcClient authClient) =>
            {
                await authClient.ForgotPasswordRequestAsync(request);

                return Results.Ok(new { Message = "E-mail sent successfully." });

            }).AllowAnonymous().WithValidation<ForgotPasswordRequest>();

            route.MapPost("ResetPassword", async ([FromBody] ResetPasswordRequest request, [FromServices] IAuthGrpcClient authClient) =>
            {
                await authClient.ResetPasswordAsync(request);
                return Results.Ok(new { Message = "Password has been reset successfully." });

            }).AllowAnonymous().WithValidation<ResetPasswordRequest>();

            route.MapGet("Logout", async (HttpContext context, [FromServices] IAuthGrpcClient authClient) =>
            {
                // Extract the JTI and User ID from the request
                string? jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                string? userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(userId))
                    return Results.BadRequest("Invalid session data.");

                await authClient.LogoutAsync(new LogoutRequest(
                    Jti: jti,
                    UserId: userId
                ));

                return Results.Ok(new { Message = "Logout successful." });

            }).RequireAuthorization();

            return route;
        }
    }
}

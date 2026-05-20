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

        private static RouteGroupBuilder MapAuthGroup(this RouteGroupBuilder group)
        {
            group.MapPost("Login", async ([FromBody] LoginRequest request, [FromServices] IAuthGrpcClient authClient) =>
            {
                LoginResponse response = await authClient.LoginAsync(request);
                return Results.Ok(response);

            }).AllowAnonymous().WithValidation<LoginRequest>();

            group.MapPost("Logout", async (HttpContext context, [FromServices] IAuthGrpcClient authClient) =>
            {
                // Extract the JTI and User ID from the request
                var jti = context.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
                var userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(jti) || string.IsNullOrWhiteSpace(userId))
                    return Results.BadRequest("Invalid session data.");

                var response = await authClient.LogoutAsync(new LogoutRequest(
                    Jti: jti,
                    UserId: userId
                ));

                return Results.Ok(new { response.Message });

            }).RequireAuthorization();

            return group;
        }
    }
}

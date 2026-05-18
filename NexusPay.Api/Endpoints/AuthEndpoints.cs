using Microsoft.AspNetCore.Mvc;
using NexusPay.Api.Extensions;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.Auth;

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
            group.MapPost("Login", async ([FromBody] LoginRequest request, [FromServices] IAuthGrpcClient service) =>
            {
                LoginResponse response = await service.LoginAsync(request);
                return Results.Ok(response);

            }).AllowAnonymous().WithValidation<LoginRequest>();

            group.MapPost("Logout", async ([FromBody] LogoutRequest request, [FromServices] IAuthGrpcClient service) =>
            {
                await service.LogoutAsync(request);
                return Results.Ok();

            }).RequireAuthorization();

            return group;
        }
    }
}

using NexusPay.Api.Extensions;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.Auth;

namespace NexusPay.Api.Endpoints
{
    public static class AuthEndpoints
    {
        public static WebApplication MapAuthEndpoints(this WebApplication app)
        {
            app.MapGroup("auth").AllowAnonymous().MapAuthGroup().WithTags("Auth");

            return app;
        }

        private static RouteGroupBuilder MapAuthGroup(this RouteGroupBuilder group)
        {
            group.MapPost("Login", async (LoginRequest request, IAuthGrpcClient service) =>
            {
                var response = await service.LoginAsync(request);
                return Results.Ok(new { response.Token });
            }).WithValidation<LoginRequest>();

            group.MapPost("Logout", async (LogoutRequest request, IAuthGrpcClient service) =>
            {
                await service.LogoutAsync(request);
                return Results.Ok();
            });

            return group;
        }
    }
}

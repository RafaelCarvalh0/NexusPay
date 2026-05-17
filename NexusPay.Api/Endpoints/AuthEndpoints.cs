using NexusPay.Client.Services;
using NexusPay.Entities.Models.Auth;

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
            });

            group.MapPost("Register", async (/*RegisterRequest request*/) =>
            {
                // Implement registration logic here
                return Results.Ok(new { Message = "User registered successfully" });
            });

            return group;
        }
    }
}

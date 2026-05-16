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
            group.MapPost("Login", (LoginRequest request, IAuthGrpcClient service) =>
            {
                var response = service.LoginAsync(request).Result;
                return Results.Ok(new { Token = response.Token });
            });

            group.MapPost("Register", (/*RegisterRequest request*/) =>
            {
                // Implement registration logic here
                return Results.Ok(new { Message = "User registered successfully" });
            });

            return group;
        }
    }
}

using NexusPay.Api.Extensions;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.User;

namespace NexusPay.Api.Endpoints
{
    public static class UserEndpoints
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            app.MapGroup("user").AllowAnonymous().MapUserGroup().WithTags("User");

            return app;
        }

        private static RouteGroupBuilder MapUserGroup(this RouteGroupBuilder group)
        {
            group.MapPost("Create", async (CreateUserRequest request, IUserGrpcClient service) =>
            {
                var response = await service.CreateUserAsync(request);
                return Results.Ok(response);
            }).WithValidation<CreateUserRequest>();

            return group;
        }
    }
}

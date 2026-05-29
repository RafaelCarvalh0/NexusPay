using Microsoft.AspNetCore.Mvc;
using NexusPay.Api.Extensions;
using NexusPay.Client.Services;
using NexusPay.Shared.Models.User;
using System.IdentityModel.Tokens.Jwt;

namespace NexusPay.Api.Endpoints
{
    public static class UserEndpoints
    {
        public static WebApplication MapUserEndpoints(this WebApplication app)
        {
            app.MapGroup("user").MapUserGroup().WithTags("User");

            return app;
        }

        private static RouteGroupBuilder MapUserGroup(this RouteGroupBuilder route)
        {
            route.MapPost("Create", async (CreateUserRequest request, IUserGrpcClient service) =>
            {
                await service.CreateUserAsync(request);
                return Results.Ok(new { Message = "User created successfully" });

            }).AllowAnonymous().WithValidation<CreateUserRequest>();

            route.MapPatch("Update", async (HttpContext context, UpdateUserRequest request, IUserGrpcClient service) =>
            {
                string? userId = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrWhiteSpace(userId))
                    return Results.Unauthorized();

                await service.UpdateUserAsync(userId, request);
                return Results.Ok(new { Message = "User updated successfully" });

            }).RequireAuthorization().WithValidation<UpdateUserRequest>();

            route.MapDelete("Delete/{userId:guid}", async ([FromRoute] Guid userId, IUserGrpcClient service) =>
            {
                if (userId == Guid.Empty)
                    return Results.BadRequest(new { Message = "Invalid user ID" });

                await service.DeleteUserAsync(userId);
                return Results.Ok(new { Message = "User deleted successfully" });

            }).RequireAuthorization("Admin");

            return route;
        }
    }
}

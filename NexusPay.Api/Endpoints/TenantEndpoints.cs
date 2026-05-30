using NexusPay.Api.Extensions;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Shared.Models.Tenant;

namespace NexusPay.Api.Endpoints
{
    public static class TenantEndpoints
    {
        public static WebApplication MapTenantEndpoints(this WebApplication app)
        {
            app.MapGroup("tenant").MapTenantGroup().WithTags("Tenant");

            return app;
        }

        private static RouteGroupBuilder MapTenantGroup(this RouteGroupBuilder route)
        {
            route.MapPost("Create", async (CreateTenantRequest request, ITenantGrpcClient service) =>
            {
                await service.CreateTenantAsync(request);
                return Results.Ok(new { Message = "Tenant created successfully" });

            }).RequireAuthorization("Admin").WithValidation<CreateTenantRequest>();

            return route;
        }
    }
}

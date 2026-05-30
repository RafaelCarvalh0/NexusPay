using NexusPay.Shared.Models.Tenant;

namespace NexusPay.Client.Services.Interfaces
{
    public interface ITenantGrpcClient
    {
        Task CreateTenantAsync(CreateTenantRequest request);
    }
}

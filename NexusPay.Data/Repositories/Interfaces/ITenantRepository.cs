using NexusPay.Shared.Models.Tenant;

namespace NexusPay.Data.Repositories.Interfaces
{
    public interface ITenantRepository
    {
        Task CreateTenant(CreateTenantRequest request);
    }
}

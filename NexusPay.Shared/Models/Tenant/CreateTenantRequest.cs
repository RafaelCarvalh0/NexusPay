namespace NexusPay.Shared.Models.Tenant
{
    public record CreateTenantRequest(string Name, string Document, string Email, string? Phone);
}

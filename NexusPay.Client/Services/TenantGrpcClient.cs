using Grpc.Net.Client;
using Microsoft.Extensions.Logging;
using NexusPay.Client.Services.Interfaces;
using NexusPay.Contracts;
using NexusPay.Shared.Models.Tenant;

namespace NexusPay.Client.Services
{
    public class TenantGrpcClient(GrpcChannel channel, ILogger<TenantGrpcClient> logger) : ITenantGrpcClient
    {
        private readonly TenantService.TenantServiceClient _client = new(channel);

        public async Task CreateTenantAsync(CreateTenantRequest request)
        {
            await _client.CreateTenantAsync(new CreateTenantGrpcRequest
            {
                Name = request.Name,
                Document = request.Document,
                Email = request.Email,
                Phone = request.Phone
            });

            logger.LogInformation("Tenant created: {Name}", request.Name);
        }
    }
}

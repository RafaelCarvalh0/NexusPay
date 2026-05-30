using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NexusPay.Contracts;
using NexusPay.Data.Repositories.Interfaces;
using NexusPay.Shared.Models.Tenant;

namespace NexusPay.Server.Services
{
    public class TenantGrpcService(ILogger<TenantGrpcService> logger, ITenantRepository tenantRepository) : TenantService.TenantServiceBase
    {
        public async override Task<Empty> CreateTenant(CreateTenantGrpcRequest request, Grpc.Core.ServerCallContext context)
        {
            logger.LogInformation("Received CreateTenant request for tenant with name: {TenantName}", request.Name);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required"));

            if (string.IsNullOrWhiteSpace(request.Document))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Document is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            await tenantRepository.CreateTenant(new CreateTenantRequest
            (
                Name: request.Name,
                Document: request.Document,
                Email: request.Email,
                Phone: request.Phone
            ));

            return new Empty();
        }
    }
}

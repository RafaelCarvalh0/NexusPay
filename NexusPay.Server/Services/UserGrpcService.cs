using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NexusPay.Contracts;
using NexusPay.Data.Repositories.Interfaces;
using NexusPay.Shared.Models.User;

namespace NexusPay.Server.Services
{
    public class UserGrpcService(ILogger<UserGrpcService> logger, IUserRepository userRepository) : UserService.UserServiceBase
    {
        public override async Task<Empty> CreateUser(CreateUserGrpcRequest request, ServerCallContext context)
        {
            logger.LogInformation("Received CreateUser request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

            await userRepository.CreateUser(new CreateUserRequest
            (
                Name: request.Name,
                Email: request.Email,
                Password: request.Password,
                RoleId: request.RoleId
            ));

            return new Empty();
        }

        public override async Task<Empty> UpdateUser(UpdateUserGrpcRequest request, ServerCallContext context)
        {
            logger.LogInformation("Received UpdateUser request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            await userRepository.UpdateUser(request.Id, new UpdateUserRequest
            (
                Name: request.Name,
                Email: request.Email
            ));

            return new Empty();
        }

        public override async Task<Empty> DeleteUser(DeleteUserGrpcRequest request, ServerCallContext context)
        {
            logger.LogInformation("Received DeleteUser request for user ID: {UserId}", request.Id);

            if (string.IsNullOrWhiteSpace(request.Id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "User ID is required"));

            if (!Guid.TryParse(request.Id, out Guid id))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid user ID format"));

            await userRepository.DeleteUser(id);

            return new Empty();
        }
    }
}

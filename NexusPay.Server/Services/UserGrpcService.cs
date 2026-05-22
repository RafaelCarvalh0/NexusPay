using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NexusPay.Contracts;
using NexusPay.Data.Repositories;
using NexusPay.Shared.Models.User;

namespace NexusPay.Server.Services
{
    public class UserGrpcService : UserService.UserServiceBase
    {
        private readonly ILogger<UserGrpcService> _logger;
        private readonly IUserRepository _userRepository;

        public UserGrpcService(ILogger<UserGrpcService> logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public override async Task<Empty> CreateUser(CreateUserGrpcRequest request, ServerCallContext context)
        {
            _logger.LogInformation("Received CreateUser request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Password is required"));

            await _userRepository.CreateUser(new CreateUserRequest
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
            _logger.LogInformation("Received UpdateUser request for email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Name))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Name is required"));

            if (string.IsNullOrWhiteSpace(request.Email))
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Email is required"));

            await _userRepository.UpdateUser(request.Id, new UpdateUserRequest
            (
                Name: request.Name,
                Email: request.Email
            ));

            return new Empty();
        }
    }
}

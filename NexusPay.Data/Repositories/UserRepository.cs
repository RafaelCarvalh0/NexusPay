using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public interface IUserRepository
    {
        Task<CreateUserGrpcResponse> CreateUser(CreateUserGrpcRequest request);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUniversal _repo;
        public UserRepository(IUniversal universal, ILogger<UserRepository> logger)
        {
            _repo = universal;
        }

        public async Task<CreateUserGrpcResponse> CreateUser(CreateUserGrpcRequest request)
        {
            string hashedPassword = PasswordHelper.Hash(request.Password);

            await _repo.ExecuteNonQueryAsync(
                command: "SP_CREATE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@USER_NAME", Value = request.Name, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@HASHED_PASSWORD", Value = hashedPassword, SqlDbType = SqlDbType.VarChar }
            );

            return new CreateUserGrpcResponse { Message = "User created successfully" };
        }
    }
}

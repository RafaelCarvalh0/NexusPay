using Grpc.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public interface IAuthRepository
    {
        Task<LoginGrpcResponse> Login(LoginGrpcRequest request);
        Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly IUniversal _repo;
        private readonly ILogger<AuthRepository> _logger;
        public AuthRepository(IUniversal universal, ILogger<AuthRepository> logger)
        {
            _repo = universal;
            _logger = logger;
        }

        public async Task<LoginGrpcResponse> Login(LoginGrpcRequest request)
        {
            var storedHash = await _repo.ExecuteScalarAsync(
               command: "SP_GET_USER_BY_EMAIL",
               type: CommandType.StoredProcedure,
               new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar }
            );

            if (storedHash is null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Email not found"));

            else if (storedHash is string && !PasswordHelper.Verify(request.Password, storedHash.ToString()!))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid password"));

            else
            {
                var result = await _repo.ExecuteDataRowAsync(
                   command: "SP_LOGIN_USER",
                   type: CommandType.StoredProcedure,
                   new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar },
                   new SqlParameter() { ParameterName = "@HASHED_PASSWORD", Value = storedHash.ToString(), SqlDbType = SqlDbType.VarChar }
                );

                var id = result["ID"].ToString();
                var name = result["USERNAME"].ToString();
                var email = result["EMAIL"].ToString();
                var createdAt = Convert.ToDateTime(result["CREATEDAT"]);
                var isActive = Convert.ToBoolean(result["ISACTIVE"]);

                return new LoginGrpcResponse
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiI5ODc2NTQzMjEiLCJuYW1lIjoiUmFmYWVsIENhcnZhbGhvIiwicm9sZSI6IkFkbWluIiwiaWF0IjoxNzQ3MzY4MDAwLCJleHAiOjE3NDczNzE2MDB9.X8mV9Kp2WfQ7LnD4sYt3AzrQvHnE5BcJuR2xZaKf91A",
                    Message = "Authentication successful"
                };
            }
        }

        public async Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

using Grpc.Core;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NexusPay.Contracts;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using NexusPay.Shared.Models.Auth.Claims;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public interface IAuthRepository
    {
        Task<AuthClaims> Login(LoginGrpcRequest request);
        Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request);
    }

    public class AuthRepository : IAuthRepository
    {
        private readonly IUniversal _repo;
        public AuthRepository(IUniversal universal)
        {
            _repo = universal;
        }

        public async Task<AuthClaims> Login(LoginGrpcRequest request)
        {
            DataRow? result = await _repo.ExecuteDataRowAsync(
               command: "SP_LOGIN_USER",
               type: CommandType.StoredProcedure,
               new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.NVarChar }
           );

            if (result is null)
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Email not found."));

            if (!PasswordHelper.Verify(request.Password, result["PASSWORDHASH"].ToString()!))
                throw new RpcException(new Status(StatusCode.Unauthenticated, "Invalid password."));

            return AuthClaims.FromDataRow(result);
        }

        public async Task<LogoutGrpcResponse> Logout(LogoutGrpcRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

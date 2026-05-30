using Grpc.Core;
using Microsoft.Data.SqlClient;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using NexusPay.Data.Repositories.Interfaces;
using NexusPay.Shared.Models.Auth;
using NexusPay.Shared.Models.Auth.Claims;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public class AuthRepository(IUniversal repo) : IAuthRepository
    {
        public async Task<AuthClaims> Login(LoginRequest request)
        {
            DataRow? result = await repo.ExecuteDataRowAsync(
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

        public async Task ResetPassword(ResetPasswordRequest resetPasswordRequest)
        {
            await repo.ExecuteNonQueryAsync(
                command: "SP_RESET_PASSWORD",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@EMAIL", Value = resetPasswordRequest.Email, SqlDbType = SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "@NEW_PASSWORD_HASH", Value = PasswordHelper.Hash(resetPasswordRequest.NewPassword), SqlDbType = SqlDbType.NVarChar }
            );
        }
    }
}

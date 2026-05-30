using Microsoft.Data.SqlClient;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using NexusPay.Data.Repositories.Interfaces;
using NexusPay.Shared.Models.User;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public class UserRepository(IUniversal repo) : IUserRepository
    {
        public async Task CreateUser(CreateUserRequest request)
        {
            string hashedPassword = PasswordHelper.Hash(request.Password);

            await repo.ExecuteNonQueryAsync(
                command: "SP_CREATE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@NAME", Value = request.Name, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@HASHED_PASSWORD", Value = hashedPassword, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@ROLE_ID", Value = request.RoleId, SqlDbType = SqlDbType.Int }
            );
        }

        public async Task DeleteUser(Guid id)
        {
            await repo.ExecuteNonQueryAsync(
                command: "SP_DELETE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@ID", Value = id, SqlDbType = SqlDbType.UniqueIdentifier }
            );
        }

        public async Task UpdateUser(string id, UpdateUserRequest request)
        {
            await repo.ExecuteNonQueryAsync(
                command: "SP_UPDATE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@ID", Value = id, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@NAME", Value = request.Name, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar }
            );
        }
    }
}

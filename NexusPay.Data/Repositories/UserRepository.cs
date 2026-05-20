using Microsoft.Data.SqlClient;
using NexusPay.Data.Configuration;
using NexusPay.Data.Helper;
using NexusPay.Shared.Models.User;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public interface IUserRepository
    {
        Task CreateUser(CreateUserRequest request);
        Task UpdateUser(string id, UpdateUserRequest request);
    }

    public class UserRepository : IUserRepository
    {
        private readonly IUniversal _repo;
        public UserRepository(IUniversal universal)
        {
            _repo = universal;
        }

        public async Task CreateUser(CreateUserRequest request)
        {
            string hashedPassword = PasswordHelper.Hash(request.Password);

            await _repo.ExecuteNonQueryAsync(
                command: "SP_CREATE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@NAME", Value = request.Name, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@HASHED_PASSWORD", Value = hashedPassword, SqlDbType = SqlDbType.VarChar }
            );
        }

        public async Task UpdateUser(string id, UpdateUserRequest request)
        {
            await _repo.ExecuteNonQueryAsync(
                command: "SP_UPDATE_USER",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@ID", Value = id, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@NAME", Value = request.Name, SqlDbType = SqlDbType.VarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.VarChar }
            );
        }
    }
}

using Microsoft.Data.SqlClient;
using NexusPay.Data.Configuration;
using NexusPay.Data.Repositories.Interfaces;
using NexusPay.Shared.Models.Tenant;
using System.Data;
using System.Security.Cryptography;

namespace NexusPay.Data.Repositories
{
    public class TenantRepository(IUniversal repo) : ITenantRepository
    {
        public async Task CreateTenant(CreateTenantRequest request)
        {
            string apiKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            await repo.ExecuteNonQueryAsync(
                command: "SP_CREATE_TENANT",
                type: CommandType.StoredProcedure,
                new SqlParameter() { ParameterName = "@NAME", Value = request.Name, SqlDbType = SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "@DOCUMENT", Value = request.Document, SqlDbType = SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "@EMAIL", Value = request.Email, SqlDbType = SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "@PHONE", Value = Utils.DBNullParse(request.Phone), SqlDbType = SqlDbType.NVarChar },
                new SqlParameter() { ParameterName = "@API_KEY", Value = apiKey, SqlDbType = SqlDbType.NVarChar }
            );
        }
    }
}

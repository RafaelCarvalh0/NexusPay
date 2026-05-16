using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using NexusPay.Data.Configuration;
using System.Data;

namespace NexusPay.Data.Repositories
{
    public interface IAuthRepository
    {
        Task Login(/*LoginRequest request*/);
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

        public async Task Login(/*LoginRequest request*/)
        {
            try
            {
                var result = await _repo.ExecuteScalarAsync(
                   command: "SP_PROCEDURE_NAME",
                   type: CommandType.StoredProcedure,
                   new SqlParameter() {  }
                   //new SqlParameter() { ParameterName = "@p_json", Value = JsonConvert.SerializeObject(autbankResponse), SqlDbType = SqlDbType.VarChar },
                   //new SqlParameter() { ParameterName = "@p_gooroo_request_perc_juros_negociado", Value = requestGooroo.dto.PercJurosNegociado, SqlDbType = SqlDbType.Decimal }
               );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during login.");
                throw;
            }
        }
    }
}

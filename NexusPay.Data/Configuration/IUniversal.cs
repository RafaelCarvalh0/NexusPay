using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace NexusPay.Data.Configuration
{
    public interface IUniversal
    {
        Task<int> ExecuteNonQueryAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters);
        Task<object> ExecuteScalarAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters);
        Task<DataTable> ExecuteDataTableAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters);
        Task<DataRow> ExecuteDataRowAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters);
    }

    public class Universal : IUniversal
    {
        private readonly ILogger<IUniversal> _logger;
        private SqlConnection _connection;
        private SqlTransaction _transaction;

        public Universal(string connectionString, ILogger<IUniversal> logger)
        {
            _logger = logger;
            _connection = new SqlConnection(connectionString);
        }

        private async Task OpenConnectionAsync()
        {
            try
            {
                if (_connection.State == ConnectionState.Closed)
                {
                    await _connection.OpenAsync();
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "OpenConnectionAsync()");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OpenConnectionAsync()");

                throw;
            }
        }

        private async Task CloseConnectionAsync()
        {
            try
            {
                if (_connection.State == ConnectionState.Open)
                {
                    await _connection.CloseAsync();
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "CloseConnectionAsync()");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CloseConnectionAsync()");

                throw;
            }
        }

        private SqlTransaction BeginTransaction()
        {
            try
            {
                _transaction = _connection.BeginTransaction();

                return _transaction;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "BeginTransaction()");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BeginTransaction()");

                throw;
            }
        }

        private async Task CancelTransactionAsync()
        {
            try
            {
                await _transaction.RollbackAsync();
                _transaction = null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "CancelTransactionAsync()");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CancelTransactionAsync()");

                throw;
            }
        }

        private async Task CommitTransactionAsync()
        {
            try
            {
                await _transaction.CommitAsync();
                _transaction = null;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "CommitTransactionAsync()");

                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CommitTransactionAsync()");

                throw;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();

                BeginTransaction();

                using (var cmd = new SqlCommand(command, _connection, _transaction))
                {
                    cmd.CommandType = type;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    var affectedRows = await cmd.ExecuteNonQueryAsync();

                    await CommitTransactionAsync();

                    return affectedRows;
                }
            }
            catch (SqlException ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteNonQueryAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                await CancelTransactionAsync();

                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteNonQueryAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                await CancelTransactionAsync();

                throw;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<object> ExecuteScalarAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();

                BeginTransaction();

                using (var cmd = new SqlCommand(command, _connection, _transaction))
                {
                    cmd.CommandType = type;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    var result = await cmd.ExecuteScalarAsync();

                    await CommitTransactionAsync();

                    return result;
                }
            }
            catch (SqlException ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteScalarAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                await CancelTransactionAsync();

                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteScalarAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                await CancelTransactionAsync();

                throw;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }


        public async Task<DataRow> ExecuteDataRowAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();

                using (var cmd = new SqlCommand(command, _connection))
                {
                    cmd.CommandType = type;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        adapter.Fill(ds);

                        return ds.Tables[0].Rows[0];
                    }
                }
            }
            catch (SqlException ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteDataRowAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteDataRowAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                throw;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        public async Task<DataTable> ExecuteDataTableAsync(string command, CommandType type = CommandType.Text, params SqlParameter[] parameters)
        {
            try
            {
                await OpenConnectionAsync();

                using (var cmd = new SqlCommand(command, _connection))
                {
                    cmd.CommandType = type;

                    if (parameters != null)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var adapter = new SqlDataAdapter(cmd))
                    {
                        var ds = new DataSet();
                        adapter.Fill(ds);

                        return ds.Tables[0];
                    }
                }
            }
            catch (SqlException ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteDataTableAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                throw;
            }
            catch (Exception ex)
            {
                var errorMessage = CreateErrorMessage("ExecuteDataTableAsync", command, type, parameters);

                _logger.LogError(ex, errorMessage, command, type);

                throw;
            }
            finally
            {
                await CloseConnectionAsync();
            }
        }

        private string CreateErrorMessage(string name, string command, CommandType type, params SqlParameter[] parameters)
        {
            try
            {
                var message = $"{name}(";
                message += "command: {command}, type: {type},";

                foreach (SqlParameter p in parameters)
                {
                    // Remove '{"' e '"}' to avoid problems with string interpolation,
                    // when logging the error message.

                    message += $" {p.ParameterName}: {p.Value.ToString()?.Replace("{\"", "").Replace("\"}", "")},";
                }

                message = message.Remove(startIndex: message.Length - 1, count: 1);
                message = message.Insert(startIndex: message.Length, value: ")");

                return message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocorreu um erro ao montar a mensagem de erro. Método: {name}", name);

                return ex.Message;
            }
        }
    }
}

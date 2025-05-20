
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

using System.Diagnostics;

namespace ScrapperEy.DataAccess
{
    public class DatabaseManager
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        //private static string _connectionString = @"data source = react.cw9aj8lxrqii.us-east-1.rds.amazonaws.com;" + "initial catalog =mitutor; user id = admin; password = irwEguLpecJujhmCnKDg";

        public DatabaseManager(IConfiguration configuration)
        {
            _configuration = configuration;

            var server = _configuration["ConnectionStrings:Server"];
            var database = _configuration["ConnectionStrings:Database"];
            var userId = _configuration["ConnectionStrings:UserId"];
            var password = _configuration["ConnectionStrings:Password"];

            _connectionString = $"data source={server};initial catalog={database};user id={userId};password={password}";
        }

        public async Task ExecuteStoredProcedure(string storedProcedureName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                    }
                    catch
                    {
                        throw new Exception("Error al ejecutar el Stored Procedure: " + storedProcedureName);
                    }
                }
            }
        }

        public async Task<DataTable> ExecuteStoredProcedureDataTable(string storedProcedureName, SqlParameter[] parameters = null)
        {
            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        await connection.OpenAsync();
                        SqlDataAdapter adapter = new SqlDataAdapter(command);
                        adapter.Fill(dataTable);
                    }
                    catch
                    {
                        throw new Exception("Error al ejecutar el Stored Procedure: " + storedProcedureName);
                    }
                }
            }
            return dataTable;
        }

        public async Task<int> ExecuteStoredProcedureWithRowsAffected(string storedProcedureName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        await connection.OpenAsync();
                        int rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error al ejecutar el Stored Procedure: {storedProcedureName}", ex);
                    }
                }
            }
        }

        public async Task<int> MeasureDatabaseResponseTimeAsync()
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand("SELECT 1", connection))
                {
                    try
                    {
                        var stopwatch = Stopwatch.StartNew();
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync();
                        stopwatch.Stop();
                        return (int)stopwatch.ElapsedMilliseconds;
                    }
                    catch
                    {
                        return -1; // Indica un error
                    }
                }
            }
        }

        public async Task<T> ExecuteStoredProcedureWithResult<T>(string storedProcedureName, SqlParameter[] parameters)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }

                    try
                    {
                        await connection.OpenAsync();
                        object result = await command.ExecuteScalarAsync();

                        if (result == DBNull.Value)
                        {
                            return default(T);
                        }

                        return (T)Convert.ChangeType(result, typeof(T));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Error al ejecutar el Stored Procedure: {storedProcedureName}", ex);
                    }
                }
            }
        }


    }
}

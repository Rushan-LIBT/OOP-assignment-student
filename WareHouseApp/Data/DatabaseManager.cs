using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WareHouseApp.Exceptions;

namespace WareHouseApp.Data
{
    /// <summary>
    /// Single point of access to the SQL Server LocalDB database.
    /// Implemented as a singleton so the connection string is read once.
    /// </summary>
    public class DatabaseManager
    {
        private static DatabaseManager instance;
        private readonly string connectionString;

        private DatabaseManager()
        {
            var setting = ConfigurationManager.ConnectionStrings["WarehouseDB"];
            if (setting == null)
            {
                throw new DataAccessException(
                    "Connection string 'WarehouseDB' is missing from App.config.",
                    new InvalidOperationException());
            }
            connectionString = setting.ConnectionString;
        }

        public static DatabaseManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DatabaseManager();
                }
                return instance;
            }
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        public int ExecuteNonQuery(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Database operation failed.", ex);
            }
        }

        public object ExecuteScalar(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    connection.Open();
                    return command.ExecuteScalar();
                }
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Database operation failed.", ex);
            }
        }

        public DataTable GetDataTable(string sql, params SqlParameter[] parameters)
        {
            try
            {
                using (var connection = GetConnection())
                using (var command = new SqlCommand(sql, connection))
                {
                    if (parameters != null)
                    {
                        command.Parameters.AddRange(parameters);
                    }
                    var table = new DataTable();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(table);
                    }
                    return table;
                }
            }
            catch (SqlException ex)
            {
                throw new DataAccessException("Database operation failed.", ex);
            }
        }
    }
}

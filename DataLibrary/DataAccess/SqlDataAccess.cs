using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace DataLibrary.DataAccess
{
    public class SqlDataAccess
    {
        private string _connectionString;

        public void GetConnectionString(string connectionString)
        {
            _connectionString = connectionString;
        }

        // A general method to retrieve data from the database.  To be used by the SeriesProcessor class in DataLibrary.
        public List<T> LoadData<T>(string sql)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                return connection.Query<T>(sql).ToList();
            }
        }

        public string RetrieveData(string sql)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                return connection.QuerySingle<string>(sql).ToString(); // Uses Dapper's QuerySingle which only retrieves one single element.
            }
        }

        public int SaveData<T>(string sql, T data)
        {
            using (IDbConnection connection = new NpgsqlConnection(_connectionString))
            {
                return connection.Execute(sql, data);
            }
        }
    }
}

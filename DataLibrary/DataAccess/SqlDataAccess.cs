using Dapper;
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

        public List<T> LoadData<T>(string sql)
        {
            using (IDbConnection connection = new SqlConnection(_connectionString))
            {
                return connection.Query<T>(sql).ToList();
            }
        }
    }
}

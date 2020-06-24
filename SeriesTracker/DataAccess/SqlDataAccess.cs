using Dapper;
using SeriesTracker.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.DataAccess
{
    public class SqlDataAccess
    {
        private string _connectionString;

        public SqlDataAccess(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<SeriesModel> LoadSeries()
        {
            string sql = "SELECT * FROM Series.ListSeries ORDER BY Title";

            var series = new List<SeriesModel>();

            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                series = db.Query<SeriesModel>(sql).ToList();
            }

            return series;
        }
    }
}

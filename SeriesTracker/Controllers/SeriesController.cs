using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeriesTracker.DataAccess;

namespace SeriesTracker.Controllers
{
    public class SeriesController : Controller
    {
        SqlDataAccess _sqlDataAccess;

        public SeriesController(IOptions<ConnectionConfig> connectionConfig)
        {
            var connection = connectionConfig.Value;
            string connectionString = connection.SeriesDB;
            _sqlDataAccess = new SqlDataAccess(connectionString);
        }

        public IActionResult Index()
        {
            var sqlDataAccess = _sqlDataAccess.LoadSeries();
            return View(sqlDataAccess);
        }
    }
}

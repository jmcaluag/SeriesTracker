using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SeriesTracker.DataAccess;
using SeriesTracker.Models;
using static DataLibrary.BusinessLogic.SeriesProcessor;

namespace SeriesTracker.Controllers
{
    public class SeriesController : Controller
    {
        string connectionString;

        public SeriesController(IOptions<ConnectionConfig> connectionConfig)
        {
            var connection = connectionConfig.Value;
            connectionString = connection.SeriesDB;
        }

        public IActionResult Index()
        {
            var data = LoadSeries(connectionString);

            List<SeriesModel> series = new List<SeriesModel>();

            foreach (var row in data)
            {
                series.Add(new SeriesModel
                {
                    Title = row.Title,
                    DebutYear = row.DebutYear,
                    FilmType = row.FilmType,
                    Genre = row.Genre,
                    Language = row.Language
                });
            }

            return View(series);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Add(SeriesModel model)
        {

            if (ModelState.IsValid)
            {
                switch(model.FilmType)
                {
                    case "0":
                        model.FilmType = "Animation";
                        break;
                    case "1":
                        model.FilmType = "Anime";
                        break;
                    case "2":
                        model.FilmType = "Live Action";
                        break;
                }

                int seriesAdded = SeriesProcessor.AddSeries(connectionString, model.Title, model.DebutYear, model.FilmType, model.Genre, model.Language);
                return RedirectToAction("Index");
            }

            return View();
        }
    }
}

﻿using System;
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
using static DataLibrary.BusinessLogic.SeasonProcessor;

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
                    SeriesID = row.SeriesID,
                    Title = row.Title,
                    DebutYear = row.DebutYear,
                    FilmType = row.FilmType,
                    Genre = row.Genre,
                    Language = row.Language
                });
            }

            return View(series);
        }

        public IActionResult AddSeries()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddSeries(SeriesModel model)
        {

            if (ModelState.IsValid)
            {
                int seriesAdded = SeriesProcessor.AddSeries(connectionString, model.Title, model.DebutYear, model.FilmType, model.Genre, model.Language);
                return RedirectToAction("Index");
            }

            return View();
        }

        public IActionResult DeleteSeries()
        {
            var data = LoadSeries(connectionString);

            List<SeriesModel> series = new List<SeriesModel>();

            foreach (var row in data)
            {
                series.Add(new SeriesModel
                {
                    SeriesID = row.SeriesID,
                    Title = row.Title,
                    DebutYear = row.DebutYear,
                    FilmType = row.FilmType,
                    Genre = row.Genre,
                    Language = row.Language
                });
            }

            return View(series);
        }
        public IActionResult ExecuteDeleteSeries(int id)
        {
            int seriesDeleted = SeriesProcessor.DeleteSeries(connectionString, id);
            return RedirectToAction("Index");
        }

        public IActionResult SeriesDetails(int? id)
        {
            var data = LoadEpisodes(connectionString, Convert.ToInt32(id)); // This returns a DataLibrary.EpisodeModel

            List<EpisodeModel> episodes = new List<EpisodeModel>();
            
            string seriesLanguage = GetSeriesLanguage(connectionString, Convert.ToInt32(id)); // Retrieves series language to decide which table to use in the view.

            if (seriesLanguage.Equals("Japanese"))
            {
                foreach (var row in data)
                {
                    episodes.Add(new EpisodeModel
                    {
                        SeasonNumber = row.SeasonNumber,
                        EpisodeID = row.EpisodeID,
                        EpisodeNumberSeries = row.EpisodeNumberSeries,
                        EpisodeNumberSeason = row.EpisodeNumberSeason,
                        TitleEnglish = row.TitleEnglish,
                        TitleRomaji = row.TitleRomaji,
                        TitleJapanese = row.TitleJapanese,
                        OriginalAirDate = row.OriginalAirDate.ToString("d")
                    });
                }
            }
            else
            {
                foreach (var row in data)
                {
                    episodes.Add(new EpisodeModel
                    {
                        SeasonNumber = row.SeasonNumber,
                        EpisodeID = row.EpisodeID,
                        EpisodeNumberSeries = row.EpisodeNumberSeries,
                        EpisodeNumberSeason = row.EpisodeNumberSeason,
                        TitleEnglish = row.TitleEnglish,
                        OriginalAirDate = row.OriginalAirDate.ToString("d")
                    });
                }
            }

            int seriesID = Convert.ToInt32(id);

            ViewData["Language"] = seriesLanguage;
            ViewData["SeriesID"] = seriesID;
            ViewData["SeriesTitle"] = GetSeriesTitle(connectionString, seriesID);

            return View(episodes);
        }

        public IActionResult AddSeason(int? id)
        {
            AddSeasonModel addSeasonModel = new AddSeasonModel()
            {
                SeriesID = Convert.ToInt32(id)
            };

            return View(addSeasonModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddSeason(AddSeasonModel model)
        {

            if(model.OneSeason.Equals("true"))
            {
                model.SpecifiedSeason = 0;
            }

            bool oneSeason;
            if(model.OneSeason.Equals("true"))
            {
                oneSeason = true;
            }
            else
            {
                oneSeason = false;
            }


            int episodesAdded = await SeasonProcessor.AddSeason(connectionString, model.WikipediaURL, oneSeason, model.SpecifiedSeason, model.SeriesID);

            return RedirectToAction("Index");
        }

        
    }
}

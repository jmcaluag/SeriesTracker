    using DataLibrary.DataAccess;
using DataLibrary.Models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.BusinessLogic
{
    public class SeriesProcessor
    {
    // Series
        public static List<SeriesModel> LoadSeries(string connectionString)
        {
            string sql = "SELECT * FROM series.view_ListSeries ORDER BY title;";

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            return sqlDataAccess.LoadData<SeriesModel>(sql);
            
        }

        public static int AddSeries(string connectionString, string title, int debutYear, string filmType, string genre, string language)
        {

            switch (filmType)
            {
                case "0":
                    filmType = "Animation";
                    break;
                case "1":
                    filmType = "Anime";
                    break;
                case "2":
                    filmType = "Live Action";
                    break;
            }

            SeriesModel series = new SeriesModel
            {
                Title = title,
                DebutYear = debutYear,
                FilmType = filmType,
                Genre = genre,
                Language = language
            };

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            string sql = @"CALL series.usp_Insert_Series(@Title, @DebutYear, @FilmType, @Genre, @Language)";

            return sqlDataAccess.SaveData<SeriesModel>(sql, series);
        }

        public static string GetSeriesLanguage(string connectionString, int seriesID)
        {
            string sql = $"SELECT * FROM series.uf_get_series_language( { seriesID } )";

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            return sqlDataAccess.RetrieveData(sql);
        }

    // Episodes
        public static List<EpisodeModel> LoadEpisodes(string connectionString)
        {
            throw new NotImplementedException();
        }
    }
}

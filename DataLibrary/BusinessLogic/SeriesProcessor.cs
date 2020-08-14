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

        public static string GetSeriesTitle(string connectionString, int seriesID)
        {
            string sql = $"SELECT * FROM series.uf_get_series_title( { seriesID } )";

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            return sqlDataAccess.RetrieveData(sql);
        }

        public static string GetSeriesLanguage(string connectionString, int seriesID)
        {
            string sql = $"SELECT * FROM series.uf_get_series_language( { seriesID } )";

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            return sqlDataAccess.RetrieveData(sql);
        }

        public static int DeleteSeries(string connectionString, int seriesID)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            string sql = $"CALL series.usp_delete_series( { seriesID } )";

            return sqlDataAccess.ExecuteStoredProcedure(sql);
        }

    // Episodes
        public static List<EpisodeModel> LoadEpisodes(string connectionString, int id)
        {
            string sqlAllEpisodes = $"SELECT * FROM series.uf_list_all_episodes( { id } )";

            SqlDataAccess sqlDataAccess = new SqlDataAccess();

            sqlDataAccess.GetConnectionString(connectionString);

            List<EpisodeModel> episodeList = TransformEpisodeModel(sqlDataAccess.LoadData<EpisodeSqlModel>(sqlAllEpisodes));

            return episodeList;
        }

        private static List<EpisodeModel> TransformEpisodeModel(List<EpisodeSqlModel> episodeSqlList)
        {
            List<EpisodeModel> episodeList = new List<EpisodeModel>();

            if(episodeSqlList.Count == 0)
            {
                return episodeList;
            }

            EpisodeModel newFirstEpisode = new EpisodeModel();
            newFirstEpisode.SeasonNumber = episodeSqlList[0].SeasonNumber;
            newFirstEpisode.EpisodeID = episodeSqlList[0].EpisodeID;
            newFirstEpisode.EpisodeNumberSeries = episodeSqlList[0].EpisodeNumberSeries;
            newFirstEpisode.EpisodeNumberSeason = episodeSqlList[0].EpisodeNumberSeason;
            newFirstEpisode.OriginalAirDate = episodeSqlList[0].OriginalAirDate;

            switch(episodeSqlList[0].LanguageCode)
            {
                case "ENG":
                    newFirstEpisode.TitleEnglish = episodeSqlList[0].Title;
                    break;                
                case "RMJ":
                    newFirstEpisode.TitleRomaji = episodeSqlList[0].Title;
                    break;                
                case "JPN":
                    newFirstEpisode.TitleJapanese = episodeSqlList[0].Title;
                    break;
            }

            episodeList.Add(newFirstEpisode);
            int currentEpisodeID = episodeSqlList[0].EpisodeID;
            int processingEpisodeNumber = 0;

            for (int i = 0; i < episodeSqlList.Count; i++)
            {
                if (episodeSqlList[i].EpisodeID != currentEpisodeID)
                {
                    EpisodeModel newEpisode = new EpisodeModel();
                    newEpisode.SeasonNumber = episodeSqlList[i].SeasonNumber;
                    newEpisode.EpisodeID = episodeSqlList[i].EpisodeID;
                    newEpisode.EpisodeNumberSeries = episodeSqlList[i].EpisodeNumberSeries;
                    newEpisode.EpisodeNumberSeason = episodeSqlList[i].EpisodeNumberSeason;
                    newEpisode.OriginalAirDate = episodeSqlList[i].OriginalAirDate;

                    switch (episodeSqlList[i].LanguageCode)
                    {
                        case "ENG":
                            newEpisode.TitleEnglish = episodeSqlList[i].Title;
                            break;
                        case "RMJ":
                            newEpisode.TitleRomaji = episodeSqlList[i].Title;
                            break;
                        case "JPN":
                            newEpisode.TitleJapanese = episodeSqlList[i].Title;
                            break;
                    }

                    episodeList.Add(newEpisode);
                    currentEpisodeID = episodeSqlList[i].EpisodeID;
                    processingEpisodeNumber++;
                }
                else
                {
                    switch (episodeSqlList[i].LanguageCode)
                    {
                        case "ENG":
                            episodeList[processingEpisodeNumber].TitleEnglish = episodeSqlList[i].Title;
                            break;
                        case "RMJ":
                            episodeList[processingEpisodeNumber].TitleRomaji = episodeSqlList[i].Title;
                            break;
                        case "JPN":
                            episodeList[processingEpisodeNumber].TitleJapanese = episodeSqlList[i].Title;
                            break;
                    }
                }
            }

            return episodeList;
        }
    }
}

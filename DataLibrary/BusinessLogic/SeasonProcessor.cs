using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DataLibrary.Models;
using static DataLibrary.BusinessLogic.WikitextScraper;
using static DataLibrary.BusinessLogic.SeriesProcessor;
using DataLibrary.DataAccess;
using System.Data;
using Npgsql;
using Dapper;

namespace DataLibrary.BusinessLogic
{
    public class SeasonProcessor
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<int> AddSeason(string connectionString, string wikipediaURL, bool oneSeason, int specifiedSeason, int seriesID)
        {
            // 1. Wikipedia URL into Wikipedia RESTful API URI

            // 2. Get sections from the Wikipedia page using the RESTful API URI
                // 2.1 If there is one season in the series
                    // 2.1.1 Find index of the section that has the episode list
                    // 2.1.2 Use the index number to retrieve the contents of the section (i.e. the episodes in wikitext format)
                    // 2.1.3 Assign the episode list wikitext to a string variable to be parsed.

                // 2.2 If there is more than one season in the series
                    // 2.2.1 Use specifiedSeason to find the section of the specified season.
                    // 2.2.2 Get the content of that section
                        // 2.2.2.1 Find the wikilink of the season in the section.
                        // 2.2.2.2 Create a URL of the wikipedia season page from the wikilink
                    // 2.2.3 Use the wikipedia season page turn into a RESTful API URI
                    // 2.2.4 Get sections from the wikipedia season page.
                    // 2.2.5 Repeate steps from part 2.1 (i.e. find the section of the episode and retrieve its wikitext.

            // 3. Wikitext of episode list of season ready to be parsed and scraped.
                // 3.1 Parse and scrape episodes from the wikitext and put into an WikiEpisode list.

            // 4. Use WikiEpisode list and loop to add episode details in the database.
                // 4.1 Add a new season record to Series.Season
                // 4.2 Retrieve season id of the season being added (from 4.1) - DON'T NEED THIS BECAUSE THE STORED PROCEDURE WAS ALREADY DESIGNED FOR THIS
                // 4.3 Retrieve language of the series to add Japanese episodes or ignore that process.
                // 4.4 Loop to add episodes from WikiEpisode and episode titles.

            // 1
            string wikipediaURI = CreateWikiURI(wikipediaURL);

            // 2
            List<Section> wikiSections = await GetListOfWikiSections(wikipediaURI);
            string episodeListAsWikitext = await GetEpisodeListAsWikitext(wikiSections, oneSeason, wikipediaURL, specifiedSeason);

            // 3
            if(oneSeason) { specifiedSeason = 1; }
            List<WikiEpisode> listOfEpisodes = ScrapeEpisodes(episodeListAsWikitext, seriesID, specifiedSeason);

            // TODO: 
            // 4
            int addedSeason = AddNewSeason(connectionString, oneSeason, seriesID, specifiedSeason); // 4.1

            string seriesLanguage = GetSeriesLanguage(connectionString, seriesID); // 4.3

            // Arguments needed to add episode details: seriesID, seasonNumber, episodeNumberSeason, episodeNumberSeries, original air date
            // Arguments needed to add episode titles: episode ID, language code, title
            int seasonID = GetSeasonID(connectionString, oneSeason, seriesID, specifiedSeason);
            int addedEpisodes = InsertEpisodesToDatabase(connectionString, seriesLanguage, seasonID, listOfEpisodes); // 4.4
            

            return addedEpisodes; // Temp to appease the return method.
        }

        private static string CreateWikiURI(string wikipediaURL)
        {
            wikipediaURL = wikipediaURL.Trim();
            string wikiUriFormat = "https://en.wikipedia.org/w/api.php?action=parse&format=json&prop=sections&page=";
            string subDirectory = GetWikiSubdirectory(wikipediaURL);

            return wikiUriFormat + subDirectory;
        }

        private static string GetWikiSubdirectory(string wikipediaURL)
        {
            int indexShift = 5; //Takes account of "wiki/" spaces.

            int number = wikipediaURL.IndexOf("wiki/") + indexShift;

            return wikipediaURL.Substring(number);
        }

        private static async Task<List<Section>> GetListOfWikiSections(string wikipediaURI)
        {
            WikiSection retrievedJsonSection = await GetWikiSectionsAsJSON(wikipediaURI);

            return retrievedJsonSection.SectionParse.Sections;
        }

        private static async Task<WikiSection> GetWikiSectionsAsJSON(string wikiURI)
        {
            string response = await client.GetStringAsync(wikiURI);
            WikiSection wikiSectionJson = JsonSerializer.Deserialize<WikiSection>(response);

            return wikiSectionJson;
        }

        private static async Task<string> GetEpisodeListAsWikitext(List<Section> wikiSections, bool oneSeason, string wikipediaURL, int specifiedSeason)
        {
            int sectionIndexOfEpisodes = 0;
            string episodeListAsWikitext = "";

            if (oneSeason)
            {
                sectionIndexOfEpisodes = GetEpisodesIndex(wikiSections);
                string episodeSectionUri = CreateUriToEpisodeList(sectionIndexOfEpisodes, GetWikiSubdirectory(wikipediaURL));
                episodeListAsWikitext = await GetContentsAsWikitext(episodeSectionUri); // Gets Episode List as Wikitext
                return episodeListAsWikitext;
            }
            else
            {
                int indexOfSpecifiedSeason = GetSeasonIndex(wikiSections, specifiedSeason);
                string seasonSectionUri = CreateUriToEpisodeList(indexOfSpecifiedSeason, GetWikiSubdirectory(wikipediaURL));
                string contentOfSeasonSection = await GetContentsAsWikitext(seasonSectionUri); // Gets the season links as Wikitext
                string seasonPageUrl = CreateSeasonPageUrl(contentOfSeasonSection);
                string seasonPageUri = CreateWikiURI(seasonPageUrl);
                List<Section> seasonPageSections = await GetListOfWikiSections(seasonPageUri);
                episodeListAsWikitext = await GetEpisodeListAsWikitext(seasonPageSections, true, seasonPageUrl, 0); // Used a recurvsive 
            }

            return episodeListAsWikitext;
        }

        private static int GetEpisodesIndex(List<Section> wikiSections)
        {
            int numberOfSections = wikiSections.Count;
            int indexPosition = 0;
            int episodeIndex = 0;

            while (indexPosition < numberOfSections)
            {
                Section section = wikiSections[indexPosition];

                if (section.Line.Equals("Episode list") || section.Line.Equals("Episodes"))
                {
                    episodeIndex = Convert.ToInt32(section.Index);
                    break;
                }

                indexPosition++;
            }

            return episodeIndex;
        }

        private static string CreateUriToEpisodeList(int indexSection, string subDirectory)
        {
            string selectedSectionURI = String.Format($"https://en.wikipedia.org/w/api.php?action=parse&format=json&page={ subDirectory }&prop=wikitext&section={ indexSection }");

            return selectedSectionURI;
        }

        private static async Task<string> GetContentsAsWikitext(string episodeSectionUri)
        {
            string response = await client.GetStringAsync(episodeSectionUri);
            WikitextSeason episodeSection= JsonSerializer.Deserialize<WikitextSeason>(response);

            string episodeList = episodeSection.SeasonParse.SeasonWikitext.Content;

            return episodeList;
        }

        private static int GetSeasonIndex(List<Section> wikiSections, int specifiedSeason)
        {
            int sectionSize = wikiSections.Count;
            int indexPos = 0;
            int seasonIndex = -1;

            while (indexPos < sectionSize)
            {
                Section section = wikiSections[indexPos];

                if (section.Line.Contains($"Season { specifiedSeason }"))
                {
                    seasonIndex = Convert.ToInt32(wikiSections[indexPos].Index);
                    break;
                }

                indexPos++;
            }

            return seasonIndex;
        }

        private static string CreateSeasonPageUrl(string contentOfSeasonSection)
        {
            Regex regexPattern = new Regex(@"{:[\w\s()]+");

            string wikipediaURL = "https://en.wikipedia.org/wiki/";

            string seasonPageURL = null;

            if (regexPattern.IsMatch(contentOfSeasonSection))
            {
                string seasonPageSubdirectory = regexPattern.Match(contentOfSeasonSection).Value.Substring(2);

                seasonPageURL = wikipediaURL + seasonPageSubdirectory.Trim().Replace(' ', '_');
            }

            return seasonPageURL;

        }

        private static int GetSeasonID(string connectionString, bool oneSeason, int seriesID, int seasonNumber)
        {
            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            sqlDataAccess.GetConnectionString(connectionString);

            string sql;

            if (oneSeason)
            {
                sql = $"SELECT * FROM Series.uf_Get_SeasonID({ seriesID }, 1)";
            }
            else
            {
                sql = $"SELECT * FROM Series.uf_Get_SeasonID({ seriesID }, { seasonNumber })";
            }

            return Convert.ToInt32(sqlDataAccess.RetrieveData(sql));
        }

        // Stored procedures from the Entertainment Database
        private static int AddNewSeason(string connectionString, bool oneSeason, int seriesID, int seasonNumber)
        {
            int addedSeason;
            if (oneSeason)
            {
                addedSeason = InsertNewSeason(connectionString, seriesID, 1);
            }
            else
            {
                addedSeason = InsertNewSeason(connectionString, seriesID, seasonNumber);
            }

            return addedSeason;
        }
        private static int InsertNewSeason(string connectionString, int seriesID, int seasonNumber)
        {
            string sql = @"CALL Series.usp_Insert_Season_No_Title(@SeriesID, @SeasonNumber)";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                return connection.Execute(sql, new { SeriesID = seriesID, SeasonNumber = seasonNumber });
            }
        }

        private static int InsertEpisodesToDatabase(string connectionString, string seriesLanguage, int seasonID, List<WikiEpisode> episodeList)
        {
            int episodesAdded = 0;
            int episodeTitlesAdded = 0;

            SqlDataAccess sqlDataAccess = new SqlDataAccess();
            sqlDataAccess.GetConnectionString(connectionString);

            string insertEpisodeSQL = @"CALL Series.usp_Insert_Episode(@SeriesID, @SeasonNumber, @episodeNumberInSeries, @episodeNumberInSeason, @originalAirDate)";

            for (int i = 0; i < episodeList.Count; i++)
            {
                WikiEpisode currentEpisode = episodeList[i];

                episodesAdded += sqlDataAccess.SaveData<WikiEpisode>(insertEpisodeSQL, currentEpisode);

                string retrieveEpisodeIDSQL = $"SELECT * FROM series.uf_retrieve_episodeid({ seasonID }, '{ episodeList[i].episodeNumberInSeason }')";
                int episodeID = Convert.ToInt32(sqlDataAccess.RetrieveData(retrieveEpisodeIDSQL));

                //Add English Episode
                episodeTitlesAdded += InsertEpisodeTitle(connectionString, episodeID, "ENG", currentEpisode.title);

                if(seriesLanguage.Equals("Japanese"))
                {
                    episodeTitlesAdded += InsertEpisodeTitle(connectionString, episodeID, "RMJ", currentEpisode.titleRomaji);
                    episodeTitlesAdded += InsertEpisodeTitle(connectionString, episodeID, "JPN", currentEpisode.titleJapanese);
                }
            }

            return episodesAdded;
        }

        private static int InsertEpisodeTitle(string connectionString, int episodeID, string languageCode, string title)
        {
            string sql = @"CALL Series.usp_Insert_Episode_Title(@EpisodeID, @LanguageCode, @Title)";

            using (IDbConnection connection = new NpgsqlConnection(connectionString))
            {
                return connection.Execute(sql, new { EpisodeID = episodeID, LanguageCode = languageCode, Title = title});
            }
        }

    }

}

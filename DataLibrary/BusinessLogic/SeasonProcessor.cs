using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.BusinessLogic
{
    public class SeasonProcessor
    {
        public static int AddSeason(string connectionString, string wikipediaURL, bool oneSeason, int specifiedSeason, int seriesID)
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

            throw new NotImplementedException();
        }
    }

}

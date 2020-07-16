using DataLibrary.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace DataLibrary.BusinessLogic
{
    public class WikitextScraper
    {
        public static List<WikiEpisode> ScrapeEpisodes(string episodeListAsWikitext)
        {
            List<WikiEpisode> listOfEpisodes = new List<WikiEpisode>();

            using (StringReader reader = new StringReader(episodeListAsWikitext))
            {
                bool collectStatus = false;

                while(reader.Peek() > -1)
                {
                    string currentLine = reader.ReadLine();

                    if (FindEpisode(currentLine))
                    {
                        listOfEpisodes.Add(new WikiEpisode());
                        collectStatus = true;
                    }
                    else if (FindEndOfEpisode(currentLine))
                    {
                        collectStatus = false;
                    }
                    else if (collectStatus)
                    {
                        if (CheckEpisodeDetail(currentLine))
                        {
                            //Ignores <hr> tags and other non-episode details under the collectStatus of "true".
                            CollectEpisodeDetails(listOfEpisodes[listOfEpisodes.Count - 1], currentLine);
                        }

                    }
                }
            }

            return listOfEpisodes;
        }

        //Finds an episode block
        private static bool FindEpisode(string wikitemplateLine)
        {
            return wikitemplateLine.Contains("{{Episode list");
        }

        //Finds the end of an episode block
        private static bool FindEndOfEpisode(string wikitemplateLine)
        {
            return wikitemplateLine.Equals("}}");
        }

        //Checks each line if a template "row" begins with '|' (pipe)
        private static bool CheckEpisodeDetail(string wikitemplateLine)
        {
            string readerLine = wikitemplateLine.Trim();
            bool  validEpisodeDetail = false;

            if (readerLine.Equals(""))
            {
                validEpisodeDetail = false;
            }
            else if (readerLine[0].Equals('|'))
            {
                validEpisodeDetail = true;
            }

            return validEpisodeDetail;
        }

        private static void CollectEpisodeDetails(WikiEpisode episode, string readerLine)
        {
            string[] partialLines = readerLine.Split('=');

            string episodeKey = ParseWikitext(partialLines[0]);
            string episodeValue = ParseWikitext(partialLines[1]);

            AssignValueToEpisode(episode, episodeKey, episodeValue);
        }

        private static string ParseWikitext(string readerLine)
        {
            string partialWikitext = readerLine.Trim();

            if (partialWikitext.Length == 0)
            {
                return null;
            }
            else if (partialWikitext[0].Equals('|')) //Looks for Episode value substring.
            {
                return readerLine.Substring(1).Trim();
            }
            else if ((partialWikitext.Contains("{{") && partialWikitext.Contains("}}")) || (partialWikitext.Contains("[[") && partialWikitext.Contains("]]")))
            {
                string wikitextValue;
                Regex pattern = new Regex(@"(?<=\[\[|\{\{).*?(?=\]\]|\}\})");

                if (partialWikitext.Contains("[[") && !partialWikitext.Contains("Start date"))
                {
                    wikitextValue = pattern.Match(partialWikitext).Value.Trim();

                    wikitextValue = ParseWikiLink(wikitextValue);
                }
                else
                {
                    wikitextValue = pattern.Match(partialWikitext).Value.Trim();
                }

                    return wikitextValue;
            }
            else
            {
                return readerLine.Trim();
            }
        }

        private static string ParseWikiLink(string readerLine)
        {
            string linkLabel = readerLine;

            if (readerLine.Contains("|"))
            {
                string[] partialDetails = readerLine.Split('|');
                linkLabel = partialDetails[1];
            }

            return linkLabel;
        }

        private static void AssignValueToEpisode(WikiEpisode episode, string episodeKey, string episodeValue)
        {
            switch (episodeKey)
            {
                case "1":
                    episode.season = episodeValue;
                    break;
                case "EpisodeNumber":
                    episode.episodeNumberInSeries = specialEpisodeNumber(episodeValue);
                    break;
                case "EpisodeNumber2":
                    episode.episodeNumberInSeason = specialEpisodeNumber(episodeValue);
                    break;
                case "Title":
                    episode.title = episodeValue;
                    break;
                case "TranslitTitle":
                    episode.titleRomaji = episodeValue;
                    break;
                case "NativeTitle":
                    episode.titleJapanese = episodeValue;
                    break;
                case "OriginalAirDate":
                    episode.originalAirDate = ParseWikiDate(episodeValue);
                    break;
                default:
                    break;
            }
        }

        private static string specialEpisodeNumber(string episodeValue)
        {
            if (episodeValue.Contains("<hr />"))
            {
                episodeValue = episodeValue.Replace("<hr />", " - ");
            }

            return episodeValue;
        }

        private static DateTime ParseWikiDate(string episodeValue) //Value being passed in has to be trimmed
        {
            //Format in the form of: Start date|2016|4|10
            string[] dateValues = episodeValue.Split('|');

            return new DateTime(Convert.ToInt32(dateValues[1]), Convert.ToInt32(dateValues[2]), Convert.ToInt32(dateValues[3]));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class EpisodeSqlModel
    {
        public int SeasonID { get; set; }
        public int SeasonNumber { get; set; }
        public int EpisodeID { get; set; }
        public string EpisodeNumberSeries { get; set; }
        public string EpisodeNumberSeason { get; set; }
        public string Title { get; set; }
        public string LanguageCode { get; set; }
        public DateTime OriginalAirDate { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class EpisodeModel
    {
        public int SeasonNumber { get; set; }
        public int EpisodeID { get; set; }
        public int EpisodeSeriesNumber { get; set; }
        public int EpisodeSeasonNumber { get; set; }
        public DateTime OriginalAirDate { get; set; }
        public string TitleEnglish { get; set; }
        public string TitleRomaji { get; set; }
        public string TitleJapanese { get; set; }
    }
}

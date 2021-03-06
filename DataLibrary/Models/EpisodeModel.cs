﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class EpisodeModel
    {
        public int SeasonNumber { get; set; }
        public int EpisodeID { get; set; }
        public string EpisodeNumberSeries { get; set; }
        public string EpisodeNumberSeason { get; set; }
        public string TitleEnglish { get; set; }
        public string TitleRomaji { get; set; }
        public string TitleJapanese { get; set; }
        public DateTime OriginalAirDate { get; set; }
    }
}

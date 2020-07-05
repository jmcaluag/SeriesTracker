using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class EpisodeModel
    {
        public int SeasonNumber { get; set; }
        public int EpisodeID { get; set; }
        [Display(Name = "Overall No.")]
        public string EpisodeNumberSeries { get; set; }
        [Display(Name = "Season No.")]
        public string EpisodeNumberSeason { get; set; }
        public string TitleEnglish { get; set; }
        public string TitleRomaji { get; set; }
        public string TitleJapanese { get; set; }
        [Display(Name = "Original Air Date")]
        public string OriginalAirDate { get; set; }

    }
}

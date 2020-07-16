using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary.Models
{
    public class WikiEpisode
    {
        public string episodeNumberInSeries { get; set; } //EpisodeNumber
        public string episodeNumberInSeason { get; set; } //EpisodeNumber2
        public string title { get; set; }
        public string titleRomaji { get; set; }
        public string titleJapanese { get; set; }
        public DateTime originalAirDate { get; set; }

        public WikiEpisode()
        {
            //Null valued
            this.titleRomaji = null;
            this.titleJapanese = null;
        }
    }
}

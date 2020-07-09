using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class AddSeasonModel
    {
        public string WikipediaURL { get; set; }
        public bool OneSeason { get; set; }
        public int SpecifiedSeason { get; set; }
        public int SeriesID { get; set; }
    }
}

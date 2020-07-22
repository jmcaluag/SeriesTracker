using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class AddSeasonModel
    {
        [Display(Name = "Enter URL of the Wikipedia \"List of ...\" Episode page or Series page:" )]
        public string WikipediaURL { get; set; }
        [Display(Name = "Does the series have only one season?: ")]
        public bool OneSeason { get; set; }
        [Display(Name = "Specify which season number:")]
        public int SpecifiedSeason { get; set; } = 0;
        public int SeriesID { get; set; }
    }
}

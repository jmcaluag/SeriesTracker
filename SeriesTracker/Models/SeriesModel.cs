using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class SeriesModel
    {
        public string Title { get; set; }
        public DateTime DebutYear { get; set; }
        public string FilmType { get; set; }
        public string Genre { get; set; }
        public string Language { get; set; }
    }
}

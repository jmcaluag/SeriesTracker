using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public enum FilmTypeEnum
    {
        Animation,
        Anime,
        [Display(Name = "Live Action")]
        LiveAction
    }
}

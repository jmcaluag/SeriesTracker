using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class SeriesModel
    {
        [Display(Name = "Series Title")]
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }

        [Display(Name = "Debut Year")]
        [Required(ErrorMessage = "Please enter a debut year")]
        public DateTime DebutYear { get; set; }

        [Display(Name = "Film Type")]
        [Required(ErrorMessage = "Please enter a film type")]
        public string FilmType { get; set; }

        [Required(ErrorMessage = "Please enter a genre")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Please enter a language")]
        public string Language { get; set; }
    }
}

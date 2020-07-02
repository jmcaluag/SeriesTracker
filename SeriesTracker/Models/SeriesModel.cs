using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SeriesTracker.Models
{
    public class SeriesModel
    {
        public int SeriesID { get; set; }
        [Display(Name = "Series Title")]
        [Required(ErrorMessage = "Please enter a title")]
        public string Title { get; set; }

        [Display(Name = "Debut Year")]
        [CustomDate]
        [Required(ErrorMessage = "Please enter a debut year")]
        public int DebutYear { get; set; }

        [Display(Name = "Film Type")]
        public string FilmType { get; set; }
        public FilmTypeEnum ListOfFilmTypes { get; set; } 

        [Required(ErrorMessage = "Please enter a genre")]
        public string Genre { get; set; }

        [Required(ErrorMessage = "Please enter a language")]
        public string Language { get; set; }
    }

    public class CustomDateAttribute : RangeAttribute
    {
        public CustomDateAttribute() : base(1920, DateTime.Now.Year) { }

        public override string FormatErrorMessage(string name)
        {
            return $"Debut year must be between 1920 and { DateTime.Now.Year }";
        }
    }
}

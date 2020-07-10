using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace DataLibrary.Models
{
    public class WikitextSeason
    {
        [JsonPropertyName("parse")]
        public SeasonParse SeasonParse { get; set; }
    }

    public class SeasonParse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("pageid")]
        public long Pageid { get; set; }

        [JsonPropertyName("wikitext")]
        public SeasonWikitext SeasonWikitext { get; set; }
    }

    public class SeasonWikitext
    {
        [JsonPropertyName("*")]
        public string Content { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicRepository.Models
{
    public class SearchResults
    {
        public string Query { get; set; }
        public List<Autor> Autors { get; set; }
        public List<Album> Albums { get; set; }
        public List<TrackDetail> Tracks { get; set; }  
    }

    public class SearchModel
    {
        [Required]
        public string Query { get; set; }
    }
}
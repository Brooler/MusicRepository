using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicRepository.Models
{
    public class AlbumViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string AutorName { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Range(0.1, 5)]
        public double Rate { get; set; }
        public List<SelectListItem> list { get; set; }
    }

    public class AlbumListViewModel
    {
        public IEnumerable<Album> Albums { get; set; }
        public PagingInfo PagingInfo { get; set; } 
    }

    public class AlbumDetails
    {
        public int Id { get; set; }
        public int AutorId { get; set; }
        public string AutorName { get; set; }
        public string AlbumName { get; set; }
        public string Description { get; set; }
        public double Rate { get; set; }
        public List<Track> tracks { get; set; } 
    }
    
}
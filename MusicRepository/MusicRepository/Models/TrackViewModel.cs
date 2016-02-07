using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MusicRepository.Models
{
    public class TrackViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string AlbumName { get; set; }
        [Range(0.1, 5)]
        public double Rate { get; set; }
        public List<SelectListItem> list { get; set; } 
    }

    public class TrackDetail
    {
        public int Id { get; set; }
        public int AlbumId { get; set; }
        public int AutorId { get; set; }
        public string TrackName { get; set; }
        public string AlbumName { get; set; }
        public string AutorName { get; set; }
        public double Rate { get; set; }
    }

    public class TrackListViewModel
    {
        public List<TrackDetail> List { get; set; }
        public PagingInfo PagingInfo { get; set; } 
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Web;

namespace MusicRepository.Models
{
    public class AutorDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Album> albums { get; set; } 
    }

    public class AutorListViewModel
    {
        public IEnumerable<Autor> Autors { get; set; } 
        public PagingInfo PagingInfo { get; set; }
    }
}
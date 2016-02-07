namespace MusicRepository.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Web.Mvc;

    public partial class MusicContext : DbContext
    {
        
        public DbSet<Autor> Autors { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; } 
        public MusicContext()
            : base("name=MusicContext")
        {
        }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
    public class Autor
    {
        [HiddenInput(DisplayValue = false)]
        public int AutorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }

    public class Album
    {
        [HiddenInput(DisplayValue = false)]
        public int AlbumId { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int AutorId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [Range(0.1, 5)]
        public double Rate { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageMimeType { get; set; }
    }

    public class Track
    {
        [HiddenInput(DisplayValue = false)]
        public int TrackId { get; set; }
        public int AlbumId { get; set; }
        [Required]
        public string Name { get; set; }
        [Range(0.1, 5)]
        public double Rate { get; set; }
    }
}

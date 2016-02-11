using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MusicRepository.Models;

namespace MusicRepository.Controllers
{
    public class SearchController : Controller
    {
        MusicContext db = new MusicContext();
        // GET: Search
        public PartialViewResult SearchBar()
        {
            SearchModel model = new SearchModel();
            return PartialView(model);
        }
        [HttpPost]
        public ActionResult SearchResult(string query)
        {
            return View(SearchResultsInitializer(query));
        }

        //________________________________________________________________
        //private methods
        private SearchResults SearchResultsInitializer(string query)
        {
            SearchResults results = new SearchResults
            {
                Query = query,
                Autors = AutorInitializer(query),
                Albums = AlbumInitializer(query),
                Tracks = TrackInitializer(query)
            };
            return results;
        }
        private List<Autor> AutorInitializer(string query)
        {
            return db.Autors.Where(p => p.Name.Contains(query)).ToList();
        }

        private List<Album> AlbumInitializer(string query)
        {
            return db.Albums.Where(p => p.Name.Contains(query)).ToList();
        }

        private List<TrackDetail> TrackInitializer(string query)
        {
            List<TrackDetail> result = new List<TrackDetail>();
            foreach (var item in db.Tracks.Where(p=>p.Name.Contains(query)))
            {
                result.Add(TrackDetailBuilder(item));
            }
            return result;
        }

        private TrackDetail TrackDetailBuilder(Track track)
        {
            TrackDetail result = new TrackDetail();
            result.Id = track.TrackId;
            result.AlbumId = track.AlbumId;
            result.TrackName = track.Name;
            result.Rate = track.Rate;
            Album album = db.Albums.Find(track.AlbumId);
            result.AlbumName = album.Name;
            result.AutorId = album.AutorId;
            Autor autor = db.Autors.Find(album.AutorId);
            result.AutorName = autor.Name;
            return result;
        }
    }
}
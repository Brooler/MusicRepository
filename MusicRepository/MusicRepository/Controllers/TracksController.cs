using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicRepository.Models;

namespace MusicRepository.Controllers
{
    public class TracksController : Controller
    {
        private MusicContext db = new MusicContext();
        private List<Album> albums;
        private TrackViewModel model;
        private int PageSize = 15;

        // GET: Tracks
        public ActionResult Index(int page=1)
        {
            IEnumerable<Track> tracks = db.Tracks.OrderBy(m=>m.TrackId).Skip((page-1)*PageSize).Take(PageSize);
            TrackListViewModel viewModel = new TrackListViewModel();
            viewModel.List = new List<TrackDetail>();
            foreach (var t in tracks)
            {
                var mod = new TrackDetail();
                mod.Id = t.TrackId;
                mod.AlbumId = t.AlbumId;
                mod.TrackName = t.Name;
                mod.Rate = t.Rate;
                Album album = db.Albums.FirstOrDefault(m => m.AlbumId == t.AlbumId);
                mod.AlbumName = album.Name;
                mod.AutorId = album.AutorId;
                Autor autor = db.Autors.FirstOrDefault(m => m.AutorId == album.AutorId);
                mod.AutorName = autor.Name;
                viewModel.List.Add(mod);
            }
            viewModel.PagingInfo=new PagingInfo
            {
                CurrentPage = page,
                ItemsPerPage = PageSize,
                TotalItems = db.Tracks.Count()
            };
            return View(viewModel);
        }

        // GET: Tracks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Track track = db.Tracks.Find(id);
            if (track == null)
            {
                return HttpNotFound();
            }
            TrackDetail mod = new TrackDetail();
            mod.Id = track.TrackId;
            mod.AlbumId = track.AlbumId;
            mod.TrackName = track.Name;
            mod.Rate = track.Rate;
            Album album = db.Albums.FirstOrDefault(m => m.AlbumId == track.AlbumId);
            mod.AlbumName = album.Name;
            mod.AutorId = album.AutorId;
            Autor autor = db.Autors.FirstOrDefault(m => m.AutorId == album.AutorId);
            mod.AutorName = autor.Name;
            return View(mod);
        }

        // GET: Tracks/Create
        public ActionResult Create(int? id = null)
        {
            if (id == null)
            {
                ViewModelConfig();
            }
            else
            {
                ViewModelConfig(null, id);
            }
            ViewBag.Title = "Create";
            return View(model);
        }

        // POST: Tracks/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,AlbumName,Rate")] TrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                Track track=new Track();
                track.Name = model.Name;
                track.Rate = model.Rate;
                Album album = db.Albums.FirstOrDefault(a => a.Name == model.AlbumName);
                track.AlbumId = album.AlbumId;
                db.Tracks.Add(track);
                db.SaveChanges();
                return RedirectToAction("Details", "Albums", new {id=track.AlbumId});
            }

            return View(model);
        }

        // GET: Tracks/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Track track = db.Tracks.Find(id);
            if (track == null)
            {
                return HttpNotFound();
            }
            ViewModelConfig(track);
            ViewBag.Title = "Edit";
            return View("Create", model);
        }

        // POST: Tracks/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,AlbumName,Rate")] TrackViewModel model)
        {
            if (ModelState.IsValid)
            {
                Track track = db.Tracks.FirstOrDefault(t=>t.TrackId==model.Id);
                track.Name = model.Name;
                track.Rate = model.Rate;
                Album album = db.Albums.FirstOrDefault(a => a.Name == model.AlbumName);
                track.AlbumId = album.AlbumId;
                db.Entry(track).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Tracks/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Track track = db.Tracks.Find(id);
            if (track == null)
            {
                return HttpNotFound();
            }
            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Track track = db.Tracks.Find(id);
            db.Tracks.Remove(track);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private void ViewModelConfig(Track track = null, int? id = null)
        {
            albums = new List<Album>();
            model=new TrackViewModel();
            model.list = new List<SelectListItem>();
            albums = db.Albums.ToList();
            foreach (var a in albums)
            {
                model.list.Add(new SelectListItem() {Text = a.Name});
            }
            if (track != null)
            {
                model.Id = track.TrackId;
                model.Name = track.Name;
                model.Rate = track.Rate;
                id = track.AlbumId;
            }
            if (id != null)
            {
                Album album = db.Albums.FirstOrDefault(a => a.AlbumId == id);
                model.AlbumName = album.Name;
            }
        }
    }
}

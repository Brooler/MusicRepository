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
        private int PageSize = 15;

        // GET: Tracks
        public ActionResult Index(int page=1)
        {
            return View(TrackListViewModelInitializer(page));
        }

        // GET: Tracks/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Tracks.Find(id) == null)
            {
                return HttpNotFound();
            }
            return View(TrackDetailViewModelInitializer((int)id));
        }

        // GET: Tracks/Create
        public ActionResult Create(int? id = null)
        {
            ViewBag.Title = "Create";
            return View(ChangeObjectViewModelInitializer(null, id));
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
                CreateNewTrack(model);
                return RedirectToAction("Details", "Albums", new {id=GetTrack(model.Id).AlbumId});
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
            if (db.Tracks.Find(id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit";
            return View("Create", ChangeObjectViewModelInitializer(GetTrack(id)));
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
                SaveObjectChanges(model);
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


        //__________________________________________________________________________
        //private methods
        private Track GetTrack(int? id)
        {
            if (id != null)
            {
                return db.Tracks.Find(id);
            }
            else
            {
                return new Track();
            }
        }

        private List<Album> GetAlbumsList()
        {
            return db.Albums.ToList();
        } 

        private TrackDetail TrackDetailViewModelInitializer(Track track)
        {
            TrackDetail result = new TrackDetail
            {
                AlbumId = track.AlbumId,
                AlbumName = db.Albums.Find(track.AlbumId).Name,
                AutorId = db.Autors.Find(db.Albums.Find(track.AlbumId).AutorId).AutorId,
                AutorName = db.Autors.Find(db.Albums.Find(track.AlbumId).AutorId).Name,
                Id = track.TrackId,
                Rate = track.Rate,
                TrackName = track.Name
            };
            return result;
        }
        private TrackDetail TrackDetailViewModelInitializer(int id)
        {
            Track track = GetTrack(id);
            TrackDetail result = new TrackDetail
            {
                AlbumId = track.AlbumId,
                AlbumName = db.Albums.Find(track.AlbumId).Name,
                AutorId = db.Autors.Find(db.Albums.Find(track.AlbumId).AutorId).AutorId,
                AutorName = db.Autors.Find(db.Albums.Find(track.AlbumId).AutorId).Name,
                Id = id,
                Rate = track.Rate,
                TrackName = track.Name
            };
            return result;
        }

        private TrackListViewModel TrackListViewModelInitializer(int page)
        {
            TrackListViewModel result = new TrackListViewModel
            {
                List = new List<TrackDetail>(),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Tracks.Count()
                }
            };
            foreach (var track in db.Tracks.OrderBy(m => m.TrackId).Skip((page - 1) * PageSize).Take(PageSize))
            {
                result.List.Add(TrackDetailViewModelInitializer(track));
            }
            return result;
        }

        private TrackViewModel ChangeObjectViewModelInitializer(Track track = null, int? id = null)
        {
            TrackViewModel result;
            if (track == null)
            {
                track = new Track();
                result = new TrackViewModel
                {
                    list = new List<SelectListItem>()
                };
            }
            else
            {
                result = new TrackViewModel
                {
                    AlbumName = db.Albums.Find(track.AlbumId).Name,
                    Id = track.TrackId,
                    list = new List<SelectListItem>(),
                    Name = track.Name,
                    Rate = track.Rate
                };
            }
            foreach (var item in GetAlbumsList())
            {
                result.list.Add(new SelectListItem {Text = item.Name});
            }
            if (id != null)
            {
                result.AlbumName = db.Albums.Find(id).Name;
            }
            return result;
        }

        private void CreateNewTrack(TrackViewModel model)
        {
            Track track = new Track
            {
                AlbumId = db.Albums.FirstOrDefault(m => m.Name == model.AlbumName).AlbumId,
                Name = model.Name,
                Rate = model.Rate,
            };
            db.Tracks.Add(track);
            db.SaveChanges();
        }

        private void SaveObjectChanges(TrackViewModel model)
        {
            Track result = GetTrack(model.Id);
            result.Name = model.Name;
            result.Rate = model.Rate;
            result.AlbumId = db.Albums.FirstOrDefault(m => m.Name == model.AlbumName).AlbumId;
            db.Entry(result).State=EntityState.Modified;
            db.SaveChanges();
        }
    }
}

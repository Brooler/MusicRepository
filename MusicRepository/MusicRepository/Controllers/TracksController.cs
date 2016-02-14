using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Configuration;
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
            return View(TrackDetailsViewModelConfig(GetTrack(id)));
        }

        // GET: Tracks/Create
        public ActionResult Create(int? id = null)
        {
            ViewBag.Title = "Create";
            return View((id==null)?EditViewModelInitializer():EditViewModelInitializer((int)id));
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
                AddItemToRepository(model);
                return RedirectToAction("Details", "Albums", new {id=db.Albums.FirstOrDefault(a=>a.Name==model.AlbumName).AlbumId});
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
            if(db.Tracks.Find(id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.Title = "Edit";
            return View("Create", EditViewModelInitializer(GetTrack(id)));
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
                EditItem(model);
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
            db.Tracks.Remove(GetTrack(id));
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


        //_______________________________________________________________________________
        //private methods
        private Track GetTrack(int? id=null)
        {
            return (id == null) ? new Track() : db.Tracks.Find(id);
        }

        private IEnumerable<Track> GetTracksList(int page)
        {
            return db.Tracks.OrderBy(a=>a.TrackId).Skip((page-1)*PageSize).Take(PageSize);
        }

        private List<Album> GetAlbumsList()
        {
            return db.Albums.ToList();
        }

        private List<SelectListItem> GetItemList()
        {
            List<SelectListItem> result = new List<SelectListItem>();
            foreach (var item in GetAlbumsList())
            {
                result.Add(new SelectListItem {Text = item.Name});
            }
            return result;
        }

        private TrackDetail TrackDetailsViewModelConfig(Track track)
        {
            TrackDetail result = new TrackDetail
            {
                AlbumId = track.AlbumId,
                AutorName = db.Autors.Find(db.Albums.Find(track.AlbumId).AutorId).Name,
                AlbumName = db.Albums.Find(track.AlbumId).Name,
                AutorId = db.Albums.Find(track.AlbumId).AutorId,
                Id = track.TrackId,
                Rate = track.Rate,
                TrackName = track.Name
            };
            return result;
        }

        private List<TrackDetail> GetTracksDetailsList(int page)
        {
            List<TrackDetail> result = new List<TrackDetail>();
            foreach (var item in GetTracksList(page))
            {
                result.Add(TrackDetailsViewModelConfig(item));
            }
            return result;
        } 

        private TrackListViewModel TrackListViewModelInitializer(int page)
        {
            TrackListViewModel result = new TrackListViewModel
            {
                List = GetTracksDetailsList(page),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Tracks.Count()
                }
            };
            return result;
        }

        private TrackViewModel EditViewModelInitializer()
        {
            TrackViewModel result = new TrackViewModel
            {
                list = GetItemList()
            };
            return result;
        }
        private TrackViewModel EditViewModelInitializer(int AlbumId)
        {
            TrackViewModel result = new TrackViewModel
            {
                AlbumName = db.Albums.Find(AlbumId).Name,
                list = GetItemList()
            };
            return result;
        }
        private TrackViewModel EditViewModelInitializer(Track track)
        {
            TrackViewModel result = new TrackViewModel
            {
                AlbumName = db.Albums.Find(track.AlbumId).Name,
                Id = track.TrackId,
                list = GetItemList(),
                Name = track.Name,
                Rate = track.Rate
            };
            return result;
        }

        private int GetAlbumId(string albumName)
        {
            return db.Albums.First(a => a.Name == albumName).AlbumId;
        }

        private void AddItemToRepository(TrackViewModel model)
        {
            Track track = new Track
            {
                AlbumId = GetAlbumId(model.AlbumName),
                Name = model.Name,
                Rate = model.Rate
            };
            db.Tracks.Add(track);
            db.SaveChanges();
        }

        private void EditItem(TrackViewModel model)
        {
            Track track = new Track
            {
                AlbumId = GetAlbumId(model.AlbumName),
                Name = model.Name,
                Rate = model.Rate,
                TrackId = model.Id
            };
            db.Entry(track).State=EntityState.Modified;
            db.SaveChanges();
        }
    }
}

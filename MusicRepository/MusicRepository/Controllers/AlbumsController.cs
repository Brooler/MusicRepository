using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MusicRepository.Models;

namespace MusicRepository.Controllers
{
    public class AlbumsController : Controller
    {
        private MusicContext db = new MusicContext();
        private int PageSize = 5;
        // GET: Albums
        public ActionResult Index(int page=1)
        {
            return View(ListViewModelInitializer(page));
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (GetAlbum((int)id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.Album = GetAlbum(id);
            return View(DetailsViewModelInitializer((int)id));
        }

        // GET: Albums/Create
        public ActionResult Create(int? Autorid=null)
        {
            ViewBag.Album = new Album();
            ViewBag.Title = "Create";
            return View(ChangeRepositoryViewModelInitializer(null, Autorid));
        }
        
        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.Title = "Edit";
            ViewBag.Album = GetAlbum(id);
            return View("Create", ChangeRepositoryViewModelInitializer(id));
        }

        // POST: Albums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AlbumViewModel model, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                ChangeRepositoryPost(model, image);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        // GET: Albums/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = GetAlbum(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            if (db.Tracks.FirstOrDefault(m=>m.AlbumId==album.AlbumId) != null)
            {
                return View("DeleteWarning", DeleteWarningViewModelInitializer((int) id));
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = GetAlbum(id);
            db.Albums.Remove(album);
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
        

        public PartialViewResult AlbumSummary(int id)
        {
            Album album = GetAlbum(id);
            ViewBag.Autor = db.Autors.Find(album.AutorId).Name;
            ViewBag.Tracks = db.Tracks.Where(m => m.AlbumId == album.AlbumId).Count();
            return PartialView(album);
        }

        public FileContentResult GetImage(int id)
        {
            Album album = db.Albums.FirstOrDefault(m => m.AlbumId == id);
            if (album != null)
            {
                return File(album.ImageData, album.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        //_________________________________________________________________________
        //private methods
        private Album GetAlbum(int? id)
        {
            if (id != null)
            {
                return db.Albums.Find(id);
            }
            else
            {
                return new Album();
            }
        }

        private List<Autor> GetAutorsList()
        {
            return db.Autors.ToList();
        }

        private List<Track> GetTracksList(int AlbumId)
        {
            return db.Tracks.Where(m => m.AlbumId == AlbumId).ToList();
        } 
        
        private AlbumListViewModel ListViewModelInitializer(int page=1)
        {
            AlbumListViewModel result = new AlbumListViewModel
            {
                Albums = db.Albums.OrderBy(m => m.AlbumId).Skip((page - 1) * PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Albums.Count()
                }
            };
            return result;
        }

        private AlbumDetails DetailsViewModelInitializer(int id)
        {
            Album album = GetAlbum((int)id);
            AlbumDetails result = new AlbumDetails
            {
                AlbumName = album.Name,
                AutorId = album.AutorId,
                AutorName = db.Autors.Find(album.AutorId).Name,
                Description = album.Description,
                Id = album.AlbumId,
                Rate = album.Rate,
                tracks = db.Tracks.Where(m => m.AlbumId == album.AlbumId).ToList()
            };
            return result;
        }

        private AlbumViewModel ChangeRepositoryViewModelInitializer(int? AlbumId, int? AutorId=null)
        {
            Album album = db.Albums.Find(AlbumId);
            AlbumViewModel result;
            if (album != null)
            {
                result = new AlbumViewModel
                {
                    AutorName = db.Autors.Find(album.AutorId).Name,
                    Description = album.Description,
                    Id = album.AlbumId,
                    Name = album.Name,
                    Rate = album.Rate,
                    list = new List<SelectListItem>()
                };
            }
            else
            {
                result = new AlbumViewModel
                {
                    list = new List<SelectListItem>()
                };
            }
            foreach (var item in GetAutorsList())
            {
                result.list.Add(new SelectListItem {Text = item.Name});
            }
            if (AutorId != null)
            {
                result.AutorName = db.Autors.Find(AutorId).Name;
            }
            return result;
        }

        private void ChangeRepositoryPost(AlbumViewModel model, HttpPostedFileBase image)
        {
            Album album = GetAlbum(model.Id);
            bool newAlbum = (album.Name==null) ? true : false;
            if (image != null)
            {
                AddImage(album, image);
            }
            album.Name = model.Name;
            album.Description = model.Description;
            album.Rate = model.Rate;
            album.AutorId = db.Autors.FirstOrDefault(a => a.Name == model.AutorName).AutorId;
            if (newAlbum)
            {
                db.Albums.Add(album);
            }
            else
            {
                db.Entry(album).State = EntityState.Modified;
            }
            db.SaveChanges();
        }

        private Album AddImage(Album album, HttpPostedFileBase image)
        {
            album.ImageMimeType = image.ContentType;
            album.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(album.ImageData, 0, image.ContentLength);
            return album;
        }

        private AlbumDetails DeleteWarningViewModelInitializer(int id)
        {
            Album album = GetAlbum(id);
            AlbumDetails result = new AlbumDetails
            {
                AlbumName = album.Name,
                AutorId = album.AutorId,
                AutorName = db.Autors.Find(album.AutorId).Name,
                Description = album.Description,
                Id=album.AlbumId,
                Rate = album.Rate,
                tracks = new List<Track>()
            };
            result.tracks = GetTracksList(id);
            return result;
        }
    }
}

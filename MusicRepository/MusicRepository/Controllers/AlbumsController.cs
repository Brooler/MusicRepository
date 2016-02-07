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
        List<Autor> autors;
        AlbumViewModel model;
        private MusicContext db = new MusicContext();
        private int PageSize = 5;
        // GET: Albums
        public ActionResult Index(int page=1)
        {
            AlbumListViewModel model = new AlbumListViewModel
            {
                Albums = db.Albums.OrderBy(m => m.AlbumId).Skip((page - 1)*PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Albums.Count()
                }
            };
            return View(model);
        }

        // GET: Albums/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            AlbumDetails model = new AlbumDetails();
            model.Id = album.AlbumId;
            model.AutorId = album.AutorId;
            model.AlbumName = album.Name;
            model.Description = album.Description;
            model.Rate = album.Rate;
            Autor autor = db.Autors.FirstOrDefault(a => a.AutorId == album.AutorId);
            model.AutorName = autor.Name;
            model.tracks = db.Tracks.Where(t => t.AlbumId == album.AlbumId).ToList();
            ViewBag.Album = album;
            return View(model);
        }

        // GET: Albums/Create
        public ActionResult Create(int? Autorid=null)
        {
            if (Autorid == null)
            {
                ViewModelConfig();
            }
            else
            {
                ViewModelConfig(null, Autorid);
            }
            ViewBag.Album = new Album();
            ViewBag.Title = "Create";
            return View(model);
        }
        
        // GET: Albums/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            ViewModelConfig(album);
            ViewBag.Title = "Edit";
            ViewBag.Album = album;
            return View("Create", model);
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
                Album album = db.Albums.FirstOrDefault(a=>a.AlbumId == model.Id);
                bool newAlbum = false;
                if (album == null)
                {
                    album = new Album();
                    newAlbum = true;
                }
                if (image != null)
                {
                    album.ImageMimeType = image.ContentType;
                    album.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(album.ImageData, 0, image.ContentLength);
                }
                album.Name = model.Name;
                album.Description = model.Description;
                album.Rate = model.Rate;

                Autor autor = new Autor();
                autor = db.Autors.FirstOrDefault(a => a.Name == model.AutorName);
                album.AutorId = autor.AutorId;
                if (newAlbum)
                {
                    db.Albums.Add(album);
                }
                else
                {
                    db.Entry(album).State = EntityState.Modified;
                }
                db.SaveChanges();
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
            Album album = db.Albums.Find(id);
            if (album == null)
            {
                return HttpNotFound();
            }
            Track track = db.Tracks.FirstOrDefault(m => m.AlbumId == album.AlbumId);
            if (track != null)
            {
                AlbumDetails mod = new AlbumDetails();
                mod.Id = album.AlbumId;
                mod.AutorId = album.AutorId;
                mod.AlbumName = album.Name;
                mod.Description = album.Description;
                mod.Rate = album.Rate;
                Autor autor = db.Autors.FirstOrDefault(m => m.AutorId == album.AutorId);
                mod.AutorName = autor.Name;
                mod.tracks = db.Tracks.Where(m => m.AlbumId == album.AlbumId).ToList();
                return View("DeleteWarning", mod);
            }
            return View(album);
        }

        // POST: Albums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Album album = db.Albums.Find(id);
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
        private void ViewModelConfig(Album album = null, int? id = null)
        {
            autors = new List<Autor>();
            model = new AlbumViewModel();
            model.list = new List<SelectListItem>();
            autors = db.Autors.ToList();
            foreach (var a in autors)
            {
                model.list.Add(new SelectListItem() { Text = a.Name });
            }
            if (album != null)
            {
                model.Id = album.AlbumId;
                model.Name = album.Name;
                model.Description = album.Description;
                model.Rate = album.Rate;
                id = album.AutorId;
            }
            if (id != null)
            {
                Autor autor = db.Autors.FirstOrDefault(a => a.AutorId == id);
                model.AutorName = autor.Name;
            }
        }

        public PartialViewResult AlbumSummary(int id)
        {
            Album album = db.Albums.FirstOrDefault(m => m.AlbumId == id);
            ViewBag.Autor = db.Autors.FirstOrDefault(m => m.AutorId == album.AutorId).Name;
            ViewBag.Tracks = db.Tracks.Where(m => m.AlbumId == album.AlbumId).ToList().Count;
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
    }
}

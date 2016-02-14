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
            if (db.Albums.Find(id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.Album = GetAlbum(id);
            return View(AlbumDetailsViewModelInitializer((int)id));
        }

        // GET: Albums/Create
        public ActionResult Create(int? Autorid=null)
        {
            ViewBag.Album = new Album();
            ViewBag.Title = "Create";
            return View((Autorid==null)? EditViewModelInitializer(): EditViewModelInitializer((int)Autorid));
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
            ViewBag.Title = "Edit";
            ViewBag.Album = album;
            return View("Create", EditViewModelInitializer(album));
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
                bool newAlbum = false;
                if (db.Albums.Find(model.Id) == null)
                {
                    newAlbum = true;
                }
                Album album = ApplyItemChanges(model);
                if (image != null)
                {
                    AddImageToAlbum(album, image);
                }
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
            if (db.Albums.Find(id) == null)
            {
                return HttpNotFound();
            }
            if (db.Tracks.FirstOrDefault(m => m.AlbumId == id) != null)
            {
                return View("DeleteWarning", AlbumDetailsViewModelInitializer((int)id));
            }
            return View(GetAlbum(id));
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


        //____________________________________________________________________
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

        private Autor GetAutor(int id)
        {
            return db.Autors.Find(id);
        }

        private List<Track> GetTracksList(int id)
        {
            return db.Tracks.Where(a => a.AlbumId == id).ToList();
        }

        private List<Autor> GetAutorsList()
        {
            return db.Autors.ToList();
        } 
        private AlbumDetails AlbumDetailsViewModelInitializer(int id)
        {
            Album album = GetAlbum(id);
            AlbumDetails result = new AlbumDetails
            {
                AlbumName = album.Name,
                AutorId = album.AutorId,
                AutorName = GetAlbum(album.AutorId).Name,
                Description = album.Description,
                Id = id,
                Rate = album.Rate,
                tracks = GetTracksList(id)
            };
            return result;
        }

        private AlbumViewModel EditViewModelInitializer(Album album)
        {
            AlbumViewModel result = new AlbumViewModel
            {
                Description = album.Description,
                Id = album.AlbumId,
                list = new List<SelectListItem>(),
                Name = album.Name,
                AutorName = GetAutor(album.AutorId).Name,
                Rate = album.Rate
            };
            foreach (var item in GetAutorsList())
            {
                result.list.Add(new SelectListItem {Text = item.Name});
            }
            return result;
        }

        private AlbumViewModel EditViewModelInitializer(int id)
        {
            AlbumViewModel result = new AlbumViewModel
            {
                list = new List<SelectListItem>(),
                AutorName = GetAutor(id).Name
            };
            foreach (var item in GetAutorsList())
            {
                result.list.Add(new SelectListItem { Text = item.Name });
            }
            return result;
        }
        private AlbumViewModel EditViewModelInitializer()
        {
            AlbumViewModel result = new AlbumViewModel
            {
                list = new List<SelectListItem>()
            };
            foreach (var item in GetAutorsList())
            {
                result.list.Add(new SelectListItem { Text = item.Name });
            }
            return result;
        }

        private void AddImageToAlbum(Album album, HttpPostedFileBase image)
        {
            album.ImageMimeType = image.ContentType;
            album.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(album.ImageData, 0, image.ContentLength);
        }

        private Album ApplyItemChanges(AlbumViewModel model)
        {
            Album result = GetAlbum(model.Id);
            if (result == null)
            {
                result=new Album();
            }
            result.Name = model.Name;
            result.Description = model.Description;
            result.Rate = model.Rate;
            result.AutorId = db.Autors.FirstOrDefault(a => a.Name == model.AutorName).AutorId;
            return result;
        }
    }
}

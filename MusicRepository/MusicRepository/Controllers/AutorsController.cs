using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using MusicRepository.Models;

namespace MusicRepository.Controllers
{
    public class AutorsController : Controller
    {
        private MusicContext db = new MusicContext();
        private int PageSize = 5;
        // GET: Autors
        public ActionResult Index(int page = 1)
        {
            return View(AutorListViewModelInitializer(page));
        }

        // GET: Autors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Autors.Find(id) == null)
            {
                return HttpNotFound();
            }
            ViewBag.Autor = GetAutor(id);
            return View(DetailsViewModelInitializer((int)id));
        }

        // GET: Autors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Autors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Autor autor, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    AddImage(autor, image);
                }
                db.Autors.Add(autor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(autor);
        }

        // GET: Autors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Autors.Find(id) == null)
            {
                return HttpNotFound();
            }
            return View(GetAutor(id));
        }

        // POST: Autors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Autor autor, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                if (image != null)
                {
                    AddImage(autor, image);
                }
                db.Entry(autor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(autor);
        }

        // GET: Autors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (db.Autors.Find(id) != null)
            {
                return View("DeleteWarning", DeleteWarningViewModelInitializer((int)id));
            }
            if (db.Autors.Find(id) == null)
            {
                return HttpNotFound();
            }
            return View(GetAutor(id));
        }

        // POST: Autors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Autor autor = db.Autors.Find(id);
            db.Autors.Remove(autor);
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

        public FileContentResult GetImage(int autorid)
        {
            Autor autor = db.Autors.FirstOrDefault(m => m.AutorId == autorid);
            if (autor != null)
            {
                return File(autor.ImageData, autor.ImageMimeType);
            }
            else
            {
                return null;
            }
        }

        public PartialViewResult AutorSummary(int id)
        {
            Autor autor = GetAutor(id);
            ViewBag.Albums = db.Albums.Where(m => m.AutorId == autor.AutorId).Count();
            return PartialView(autor);
        }



        //_____________________________________________________________________________________
        //private methods
        private Autor GetAutor(int? id)
        {
            if (id != null)
            {
                return db.Autors.Find(id);
            }
            else
            {
                return new Autor();
            }
        }

        private List<Album> GetAlbums(int AutorId)
        {
            return db.Albums.Where(m=>m.AutorId==AutorId).ToList();
        } 
        private AutorListViewModel AutorListViewModelInitializer(int page)
        {
            AutorListViewModel result = new AutorListViewModel
            {
                Autors = db.Autors.OrderBy(m => m.AutorId).Skip((page - 1)*PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Autors.Count()
                }
            };
            return result;
        }

        private AutorDetails DetailsViewModelInitializer(int id)
        {
            Autor autor = GetAutor(id);
            AutorDetails result = new AutorDetails
            {
                albums = new List<Album>(),
                Description = autor.Description,
                Id = id,
                Name = autor.Name
            };
            result.albums = GetAlbums(id);
            return result;
        }

        private Autor AddImage(Autor autor, HttpPostedFileBase image)
        {
            autor.ImageMimeType = image.ContentType;
            autor.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(autor.ImageData, 0, image.ContentLength);
            return autor;
        }

        private AutorDetails DeleteWarningViewModelInitializer(int id)
        {
            Autor autor = GetAutor(id);
            AutorDetails result = new AutorDetails
            {
                albums = GetAlbums(id),
                Description = autor.Description,
                Id = id,
                Name = autor.Name
            };
            return result;
        }
    }
}

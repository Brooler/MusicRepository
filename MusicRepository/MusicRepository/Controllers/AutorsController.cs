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
        private int PageSize=5;
        // GET: Autors
        public ActionResult Index(int page=1)
        {
            AutorListViewModel model = new AutorListViewModel
            {
                Autors = db.Autors.OrderBy(m => m.AutorId).Skip((page - 1)*PageSize).Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = db.Autors.Count()
                }
            };
            return View(model);
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
            return View(AutorDetailsViewModelInitializer((int)id));
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
                    AddAutorImage(autor, image);
                }
                AddItemToRepository(autor);
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
                    AddAutorImage(autor, image);
                }
                SaveItemChanges(autor);
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
            if (db.Albums.FirstOrDefault(a=>a.AutorId==id) != null)
            {
                return View("DeleteWarning", AutorDetailsViewModelInitializer((int)id));
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
            DeleteItem(id);
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
            int count = db.Albums.Where(m => m.AutorId == id).Count();
            ViewBag.Albums = count;
            return PartialView(GetAutor(id));
        }

        //_________________________________________________________________________________
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

        private List<Album> GetAlbumsList(int autorId)
        {
            return db.Albums.Where(a => a.AutorId == autorId).ToList();
        }

        private AutorDetails AutorDetailsViewModelInitializer(int id)
        {
            Autor autor = GetAutor(id);
            AutorDetails result = new AutorDetails
            {
                albums = GetAlbumsList(id),
                Description = autor.Description,
                Id = id,
                Name = autor.Name
            };
            return result;
        }

        private void AddAutorImage(Autor autor, HttpPostedFileBase image)
        {
            autor.ImageMimeType = image.ContentType;
            autor.ImageData = new byte[image.ContentLength];
            image.InputStream.Read(autor.ImageData, 0, image.ContentLength);
        }
        private void AddItemToRepository(Autor autor)
        {
            db.Autors.Add(autor);
            db.SaveChanges();
        }

        private void SaveItemChanges(Autor autor)
        {
            db.Entry(autor).State=EntityState.Modified;
            db.SaveChanges();
        }

        private void DeleteItem(int id)
        {
            Autor autor = GetAutor(id);
            db.Autors.Remove(autor);
            db.SaveChanges();
        }
    }
}

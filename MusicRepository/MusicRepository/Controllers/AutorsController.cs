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
            Autor autor = db.Autors.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }
            AutorDetails model = new AutorDetails();
            model.Id = autor.AutorId;
            model.Name = autor.Name;
            model.Description = autor.Description;
            model.albums = db.Albums.Where(a => a.AutorId == autor.AutorId).ToList();
            ViewBag.Autor = autor;
            return View(model);
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
                    autor.ImageMimeType = image.ContentType;
                    autor.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(autor.ImageData, 0, image.ContentLength);
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
            Autor autor = db.Autors.Find(id);
            if (autor == null)
            {
                return HttpNotFound();
            }
            return View(autor);
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
                    autor.ImageMimeType = image.ContentType;
                    autor.ImageData = new byte[image.ContentLength];
                    image.InputStream.Read(autor.ImageData, 0, image.ContentLength);
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
            Autor autor = db.Autors.Find(id);
            Album album = db.Albums.FirstOrDefault(m => m.AutorId == autor.AutorId);
            if (album != null)
            {
                AutorDetails mod=new AutorDetails();
                mod.Id = autor.AutorId;
                mod.Name = autor.Name;
                mod.Description = autor.Description;
                mod.albums = db.Albums.Where(m => m.AutorId == autor.AutorId).ToList();
                return View("DeleteWarning", mod);
            }
            if (autor == null)
            {
                return HttpNotFound();
            }
            return View(autor);
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
            Autor autor = db.Autors.FirstOrDefault(m => m.AutorId == id);
            int count = db.Albums.Where(m => m.AutorId == autor.AutorId).ToList().Count;
            ViewBag.Albums = count;
            return PartialView(autor);
        }
    }
}

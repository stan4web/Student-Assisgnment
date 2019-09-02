using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using System.Web.Helpers;
using System.IO;

namespace MicroAssignment.Areas.MicroAdmin.Controllers
{
    public class AdvertController : Controller
    {
        private MicroContext db = new MicroContext();
        System.Random randomInteger = new System.Random();
        //
        // GET: /MicroAdmin/Advert/

        public ActionResult Index()
        {
            return View(db.Advertising.ToList());
        }

        //
        // GET: /MicroAdmin/Advert/Details/5

        public ActionResult Details(int id = 0)
        {
            Advertise advertise = db.Advertising.Find(id);
            if (advertise == null)
            {
                return HttpNotFound();
            }
            return View(advertise);
        }

        //
        // GET: /MicroAdmin/Advert/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MicroAdmin/Advert/Create

        [HttpPost]
        public ActionResult Create(Advertise advertise)
        {
            int genNumber = randomInteger.Next(1234567890);
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file.ContentLength > 0 && file.ContentType.ToUpper().Contains("JPEG") || file.ContentType.ToUpper().Contains("PNG"))
                        {

                            WebImage img = new WebImage(file.InputStream);
                            if (img.Width > 400)
                            {
                                img.Resize(400, 400, true, true);
                            }
                            string fileName = Path.Combine(Server.MapPath("~/Uploads/Adverts/"), Path.GetFileName(genNumber + file.FileName));
                            img.Save(fileName);
                            advertise.ImageUrl = fileName;
                        }
                        else
                        {
                            
                            return View(advertise);
                        }

                    }
                }
                catch (Exception e)
                {

                }
                db.Advertising.Add(advertise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(advertise);
        }

        //
        // GET: /MicroAdmin/Advert/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Advertise advertise = db.Advertising.Find(id);
            if (advertise == null)
            {
                return HttpNotFound();
            }
            return View(advertise);
        }

        //
        // POST: /MicroAdmin/Advert/Edit/5

        [HttpPost]
        public ActionResult Edit(Advertise advertise)
        {
            if (ModelState.IsValid)
            {
                db.Entry(advertise).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(advertise);
        }

        //
        // GET: /MicroAdmin/Advert/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Advertise advertise = db.Advertising.Find(id);
            if (advertise == null)
            {
                return HttpNotFound();
            }
            return View(advertise);
        }

        //
        // POST: /MicroAdmin/Advert/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Advertise advertise = db.Advertising.Find(id);
            db.Advertising.Remove(advertise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        
    }
}
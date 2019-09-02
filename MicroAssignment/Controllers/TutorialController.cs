using MicroAssignment.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroAssignment.Controllers
{
    public class TutorialController : Controller
    {
        private MicroContext db = new MicroContext();
        //
        // GET: /Tutorial/

        public ActionResult Index(int id)
        {
            ViewBag.Tutorial = "active";
            var tutorial = db.Tutorials.Where(p=>p.TutorialId==id).OrderByDescending(x => x.TutorialId).ToList();
            ViewBag.CategoryName = db.Categories.FirstOrDefault(x => x.CategoryId == id).CategoryName;
            return View(tutorial);
        }

        public ActionResult Category()
        {
            ViewBag.Tutorial = "active";
            var category = db.Categories.OrderBy(x => x.CategoryName).ToList();

            return View(category);
        }

        //
        // GET: /MicroAdmin/TutorialPage/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.Tutorial = "active";
            ViewBag.Id = id;
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            return View(tutorial);
        }

        //
        // GET: /MicroAdmin/TutorialPage/Create

        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /MicroAdmin/TutorialPage/Create

        [HttpPost]
        public ActionResult Create(Tutorial tutorial)
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
            if (ModelState.IsValid)
            {
                tutorial.CourseName = tutorial.CourseName.ToUpper();
                tutorial.Topic = tutorial.Topic.ToUpper();
                tutorial.UserId = userDetails.UserId;
                tutorial.Date = DateTime.Now;
                db.Tutorials.Add(tutorial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", tutorial.CategoryId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", tutorial.UserId);
            return View(tutorial);
        }

        //
        // GET: /MicroAdmin/TutorialPage/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", tutorial.CategoryId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", tutorial.UserId);
            return View(tutorial);
        }

        //
        // POST: /MicroAdmin/TutorialPage/Edit/5

        [HttpPost]
        public ActionResult Edit(Tutorial tutorial)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tutorial).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", tutorial.CategoryId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", tutorial.UserId);
            return View(tutorial);
        }

    }
}

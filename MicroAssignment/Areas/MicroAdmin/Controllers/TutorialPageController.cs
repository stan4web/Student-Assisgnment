using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using System.Text.RegularExpressions;
using PagedList;

namespace MicroAssignment.Areas.MicroAdmin.Controllers
{
    public class TutorialPageController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/TutorialPage/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "TutorialId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "TutorialId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var tutorials = from s in db.Tutorials.OrderByDescending(x => x.TutorialId).Include(t => t.Category).Include(t => t.UserProfile)
                       select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                tutorials = tutorials.Where(s => s.Topic.ToUpper().Contains(searchString.ToUpper())
                    || s.CourseName.ToUpper().Contains(searchString.ToUpper())
                    || s.Category.CategoryName.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "TutorialId_desc":
                    tutorials = tutorials.OrderByDescending(x => x.TutorialId);
                    break;
                default:
                    tutorials = tutorials.OrderBy(x => x.UserId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(tutorials.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /MicroAdmin/TutorialPage/Details/5

        public ActionResult Details(int id = 0)
        {
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

                string textHtml = HttpUtility.HtmlDecode(tutorial.Content);
                textHtml = Regex.Replace(textHtml, @"<DIV>", "<P>", RegexOptions.IgnoreCase);
                textHtml = Regex.Replace(textHtml, @"</DIV>", "</P>", RegexOptions.IgnoreCase);
                tutorial.Content = textHtml;

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

        //
        // GET: /MicroAdmin/TutorialPage/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            if (tutorial == null)
            {
                return HttpNotFound();
            }
            return View(tutorial);
        }

        //
        // POST: /MicroAdmin/TutorialPage/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Tutorial tutorial = db.Tutorials.Find(id);
            db.Tutorials.Remove(tutorial);
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
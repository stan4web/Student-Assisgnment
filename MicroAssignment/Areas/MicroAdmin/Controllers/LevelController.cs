using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using PagedList;

namespace MicroAssignment.Areas.MicroAdmin.Controllers
{
    public class LevelController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/Level/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "LevelId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "LevelId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var level = from s in db.Levels.OrderBy(x => x.LevelName)
                             select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                level = level.Where(s => s.LevelName.ToUpper().Contains(searchString.ToUpper())
                    || s.Program.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "DepartmentId_desc":
                    level = level.OrderByDescending(x => x.LevelId);
                    break;
                default:
                    level = level.OrderBy(x => x.LevelId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(level.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /MicroAdmin/Level/Details/5

        public ActionResult Details(int id = 0)
        {
            Level level = db.Levels.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // GET: /MicroAdmin/Level/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MicroAdmin/Level/Create

        [HttpPost]
        public ActionResult Create(Level level)
        {
            if (ModelState.IsValid)
            {
                db.Levels.Add(level);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(level);
        }

        //
        // GET: /MicroAdmin/Level/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Level level = db.Levels.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // POST: /MicroAdmin/Level/Edit/5

        [HttpPost]
        public ActionResult Edit(Level level)
        {
            if (ModelState.IsValid)
            {
                db.Entry(level).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(level);
        }

        //
        // GET: /MicroAdmin/Level/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Level level = db.Levels.Find(id);
            if (level == null)
            {
                return HttpNotFound();
            }
            return View(level);
        }

        //
        // POST: /MicroAdmin/Level/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Level level = db.Levels.Find(id);
            db.Levels.Remove(level);
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
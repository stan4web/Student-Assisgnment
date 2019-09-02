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
    public class AdminAssignmentController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/Assignment/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "AssignmentId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "AssignmentId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var assignments = from s in db.Assignments.Include(a => a.Department).Include(a => a.Levels).Include(a => a.UserProfile)
                       select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                assignments = assignments.Where(s => s.CourseName.ToUpper().Contains(searchString.ToUpper())
                    || s.Department.DepartmentName.Contains(searchString.ToUpper())
                    || s.Content.Contains(searchString.ToUpper())
                    || s.CourseCode.Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "AssignmentId_desc":
                    assignments = assignments.OrderByDescending(x => x.AssignmentId);
                    break;
                default:
                    assignments = assignments.OrderBy(x => x.AssignmentId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(assignments.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /MicroAdmin/Assignment/Details/5

        public ActionResult Details(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // GET: /MicroAdmin/Assignment/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /MicroAdmin/Assignment/Create

        [HttpPost]
        public ActionResult Create(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Assignments.Add(assignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", assignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", assignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", assignment.UserId);
            return View(assignment);
        }

        //
        // GET: /MicroAdmin/Assignment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", assignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", assignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", assignment.UserId);
            return View(assignment);
        }

        //
        // POST: /MicroAdmin/Assignment/Edit/5

        [HttpPost]
        public ActionResult Edit(Assignment assignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(assignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", assignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", assignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", assignment.UserId);
            return View(assignment);
        }

        //
        // GET: /MicroAdmin/Assignment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            return View(assignment);
        }

        //
        // POST: /MicroAdmin/Assignment/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Assignment assignment = db.Assignments.Find(id);
            db.Assignments.Remove(assignment);
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using PagedList;

namespace MicroAssignment.Areas.Portal.Controllers
{
    public class CourseManagementController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /Portal/CourseManagement/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "CourseID_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "CourseID_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var course = from s in db.Course.OrderBy(x => x.CourseName).Include(t => t.Session).Include(t => t.Department)
                             select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                course = course.Where(s => s.CourseName.ToUpper().Contains(searchString.ToUpper())
                    || s.CourseCode.ToUpper().Contains(searchString.ToUpper())
                    || s.Department.DepartmentName.ToUpper().Contains(searchString.ToUpper())
                    || s.Department.DepartmentCode.ToUpper().Contains(searchString.ToUpper())
                    || s.Session.SessionYear.ToUpper().Contains(searchString.ToUpper())
                    || s.Session.Semester.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "CourseID_desc":
                    course = course.OrderByDescending(x => x.CourseID);
                    break;
                default:
                    course = course.OrderBy(x => x.CourseID);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(course.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Portal/CourseManagement/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = db.Course.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // GET: /Portal/CourseManagement/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentId", "DepartmentInfo");
            ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession");
            return View();
        }

        //
        // POST: /Portal/CourseManagement/Create

        [HttpPost]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Course.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentId", "DepartmentInfo", course.DepartmentID);
            ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", course.SessionId);
            return View(course);
        }

        //
        // GET: /Portal/CourseManagement/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Course course = db.Course.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentId", "DepartmentInfo", course.DepartmentID);
            ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", course.SessionId);
            return View(course);
        }

        //
        // POST: /Portal/CourseManagement/Edit/5

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentId", "DepartmentInfo", course.DepartmentID);
            ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", course.SessionId);
            return View(course);
        }

        //
        // GET: /Portal/CourseManagement/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Course course = db.Course.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Portal/CourseManagement/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Course.Find(id);
            db.Course.Remove(course);
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
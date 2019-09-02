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
    public class InstitutionsController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/Institutions/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "SchoolId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "SchoolId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var school = from s in db.Schools.OrderByDescending(x => x.SchoolId).Include(t => t.Departments)
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                school = school.Where(s => s.SchoolName.ToUpper().Contains(searchString.ToUpper())
                    || s.SchoolCode.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "SchoolId_desc":
                    school = school.OrderByDescending(x => x.SchoolId);
                    break;
                default:
                    school = school.OrderBy(x => x.SchoolId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(school.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /MicroAdmin/Institutions/Details/5

        public ActionResult Details(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        //
        // GET: /MicroAdmin/Institutions/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /MicroAdmin/Institutions/Create

        [HttpPost]
        public ActionResult Create(School school)
        {
            if (ModelState.IsValid)
            {
                db.Schools.Add(school);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(school);
        }

        //
        // GET: /MicroAdmin/Institutions/Edit/5

        public ActionResult Edit(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        //
        // POST: /MicroAdmin/Institutions/Edit/5

        [HttpPost]
        public ActionResult Edit(School school)
        {
            if (ModelState.IsValid)
            {
                db.Entry(school).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(school);
        }

        //
        // GET: /MicroAdmin/Institutions/Delete/5

        public ActionResult Delete(int id = 0)
        {
            School school = db.Schools.Find(id);
            if (school == null)
            {
                return HttpNotFound();
            }
            return View(school);
        }

        //
        // POST: /MicroAdmin/Institutions/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            School school = db.Schools.Find(id);
            db.Schools.Remove(school);
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
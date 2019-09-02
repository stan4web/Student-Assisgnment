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
    public class DepartmentController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/Department/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "DepartmentId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "DepartmentId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var department = from s in db.Departments.OrderBy(x => x.DepartmentName).Include(t => t.School)
                         select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                department = department.Where(s => s.DepartmentName.ToUpper().Contains(searchString.ToUpper())
                    || s.DepartmentCode.ToUpper().Contains(searchString.ToUpper())
                    || s.School.SchoolName.ToUpper().Contains(searchString.ToUpper())
                    || s.School.SchoolCode.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "DepartmentId_desc":
                    department = department.OrderByDescending(x => x.DepartmentId);
                    break;
                default:
                    department = department.OrderBy(x => x.SchoolId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(department.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /MicroAdmin/Department/Details/5

        public ActionResult Details(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /MicroAdmin/Department/Create

        public ActionResult Create()
        {
            ViewBag.SchoolId=new SelectList(db.Schools.OrderBy(x=>x.SchoolName),"SchoolId","SchoolName");
            return View();
        }

        //
        // POST: /MicroAdmin/Department/Create

        [HttpPost]
        public ActionResult Create(Department department)
        {
            var schoolInfo = db.Schools.FirstOrDefault(x => x.SchoolId == department.SchoolId);
            if (ModelState.IsValid)
            {
                
                department.SchoolCode = schoolInfo.SchoolCode;

                db.Departments.Add(department);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName",department.SchoolId);
            return View(department);
        }

        //
        // GET: /MicroAdmin/Department/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /MicroAdmin/Department/Edit/5

        [HttpPost]
        public ActionResult Edit(Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        //
        // GET: /MicroAdmin/Department/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // POST: /MicroAdmin/Department/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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
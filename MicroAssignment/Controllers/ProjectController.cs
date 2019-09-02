using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using PagedList;

namespace MicroAssignment.Controllers
{
    [Authorize(Roles = "Student,Instructor,SuperAdmin")]
    public class ProjectController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /Project/
      
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.Project = "active-menu";
                ViewBag.CurrentSort = sortOrder;
                ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "MaterialId_desc" : "";
                ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "MaterialId_desc" : "";
                if (searchString != null)
                {
                    page = 1;
                }
                else
                {
                    searchString = currentFilter;
                }

                ViewBag.CurrentFilter = searchString;

                var materials = from s in db.Materials.Include(m => m.Department).Include(m => m.UserProfile).OrderByDescending(x=>x.MaterialId)
                                select s;

                if (!String.IsNullOrEmpty(searchString))
                {
                    materials = materials.Where(s => s.Description.ToUpper().Contains(searchString.ToUpper())
                        || s.Department.DepartmentName.Contains(searchString.ToUpper())
                        || s.CategoryName.Contains(searchString.ToUpper()));
                }

                switch (sortOrder)
                {
                    case "MaterialId_desc":
                        materials = materials.OrderByDescending(x => x.MaterialId);
                        break;
                    default:
                        materials = materials.OrderBy(x => x.MaterialId);
                        break;

                }

                int pageSize = 100;
                int pageNumber = (page ?? 1);
                return View(materials.ToPagedList(pageNumber, pageSize));
            
        }

        //
        // GET: /Project/Details/5
     
        public ActionResult Details(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // GET: /Project/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /Project/Create

        [HttpPost]
        public ActionResult Create(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Materials.Add(material);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", material.UserId);
            return View(material);
        }

        //
        // GET: /Project/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", material.UserId);
            return View(material);
        }

        //
        // POST: /Project/Edit/5

        [HttpPost]
        public ActionResult Edit(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", material.UserId);
            return View(material);
        }

        //
        // GET: /Project/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            return View(material);
        }

        //
        // POST: /Project/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Material material = db.Materials.Find(id);
            db.Materials.Remove(material);
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
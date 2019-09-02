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
    public class RepliedAssginmentController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /MicroAdmin/RepliedAssginment/

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

            var replyassignments = from s in db.ReplyAssignments.Include(r => r.Assignment).Include(r => r.Department).Include(r => r.Levels).Include(r => r.UserProfile)
                              select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                replyassignments = replyassignments.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper())
                    || s.Department.DepartmentName.Contains(searchString.ToUpper())
                    || s.Assignment.CourseCode.Contains(searchString.ToUpper())
                    || s.Assignment.CourseName.Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "AssignmentId_desc":
                    replyassignments = replyassignments.OrderByDescending(x => x.ReplyAssignmentId);
                    break;
                default:
                    replyassignments = replyassignments.OrderBy(x => x.ReplyAssignmentId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(replyassignments.ToPagedList(pageNumber, pageSize));
        }


        //
        // GET: /MicroAdmin/RepliedAssginment/Details/5

        public ActionResult Details(int id = 0)
        {
            ReplyAssignment replyassignment = db.ReplyAssignments.Find(id);
            if (replyassignment == null)
            {
                return HttpNotFound();
            }
            return View(replyassignment);
        }

        //
        // GET: /MicroAdmin/RepliedAssginment/Create

        public ActionResult Create()
        {
            ViewBag.AssignmentId = new SelectList(db.Assignments, "AssignmentId", "CourseName");
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /MicroAdmin/RepliedAssginment/Create

        [HttpPost]
        public ActionResult Create(ReplyAssignment replyassignment)
        {
            if (ModelState.IsValid)
            {
                db.ReplyAssignments.Add(replyassignment);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AssignmentId = new SelectList(db.Assignments, "AssignmentId", "CourseName", replyassignment.AssignmentId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", replyassignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", replyassignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", replyassignment.UserId);
            return View(replyassignment);
        }

        //
        // GET: /MicroAdmin/RepliedAssginment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ReplyAssignment replyassignment = db.ReplyAssignments.Find(id);
            if (replyassignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.AssignmentId = new SelectList(db.Assignments, "AssignmentId", "CourseName", replyassignment.AssignmentId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", replyassignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", replyassignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", replyassignment.UserId);
            return View(replyassignment);
        }

        //
        // POST: /MicroAdmin/RepliedAssginment/Edit/5

        [HttpPost]
        public ActionResult Edit(ReplyAssignment replyassignment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(replyassignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AssignmentId = new SelectList(db.Assignments, "AssignmentId", "CourseName", replyassignment.AssignmentId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", replyassignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "LevelName", replyassignment.LevelId);
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", replyassignment.UserId);
            return View(replyassignment);
        }

        //
        // GET: /MicroAdmin/RepliedAssginment/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ReplyAssignment replyassignment = db.ReplyAssignments.Find(id);
            if (replyassignment == null)
            {
                return HttpNotFound();
            }
            return View(replyassignment);
        }

        //
        // POST: /MicroAdmin/RepliedAssginment/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ReplyAssignment replyassignment = db.ReplyAssignments.Find(id);
            db.ReplyAssignments.Remove(replyassignment);
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
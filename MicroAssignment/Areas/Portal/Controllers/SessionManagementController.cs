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
    [Authorize(Roles = "SuperAdmin")]
    public class SessionManagementController : Controller
    {
        private MicroContext db = new MicroContext();

        //
        // GET: /Portal/SessionManagement/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "SessionId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "SessionId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var session = from s in db.Sessions.OrderBy(x => x.SessionYear)
                         select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                session = session.Where(s => s.SessionYear.ToUpper().Contains(searchString.ToUpper())
                    || s.Semester.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "SessionId_desc":
                    session = session.OrderByDescending(x => x.SessionId);
                    break;
                default:
                    session = session.OrderBy(x => x.SessionId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(session.ToPagedList(pageNumber, pageSize));

        }

        //
        // GET: /Portal/SessionManagement/Details/5

        public ActionResult Details(int id = 0)
        {
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        //
        // GET: /Portal/SessionManagement/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Portal/SessionManagement/Create

        [HttpPost]
        public ActionResult Create(Session session)
        {
            if (ModelState.IsValid)
            {
                db.Sessions.Add(session);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(session);
        }

        //
        // GET: /Portal/SessionManagement/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        //
        // POST: /Portal/SessionManagement/Edit/5

        [HttpPost]
        public ActionResult Edit(Session session)
        {
            if (ModelState.IsValid)
            {
                db.Entry(session).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(session);
        }

        //
        // GET: /Portal/SessionManagement/Delete/5

        public ActionResult Delete(int id = 0)
        {
            Session session = db.Sessions.Find(id);
            if (session == null)
            {
                return HttpNotFound();
            }
            return View(session);
        }

        //
        // POST: /Portal/SessionManagement/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            Session session = db.Sessions.Find(id);
            db.Sessions.Remove(session);
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
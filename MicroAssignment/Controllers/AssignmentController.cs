using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.IO;

namespace MicroAssignment.Controllers
{
    public class AssignmentController : Controller
    {
        private MicroContext db = new MicroContext();
        System.Random randomInteger = new System.Random();
        //
        // GET: /Assignment/

        public ActionResult Index()
        {
            var assignments = db.Assignments.Include(a => a.Department).Include(a => a.Levels);
            return View(assignments.ToList());
        }

        //
        // GET: /Assignment/Details/5

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
        // GET: /Assignment/Create

        public ActionResult Create()
        {
            ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName");
            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x=>x.DepartmentName), "DepartmentId", "FullDepartment");
            ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x=>x.LevelName), "LevelId", "FullLevel");
            return View();
        }

        //
        // POST: /Assignment/Create

        [HttpPost]
        public ActionResult Create(Assignment assignment, HttpPostedFileBase file)
        {
            var userdetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
            int genNumber = randomInteger.Next(1234567890);

            if (ModelState.IsValid)
            {
                try
                {
                    if (file == null)
                    {
                        ModelState.AddModelError("File", "Please upload file");
                        TempData["Error"] = "Please upload file";
                        ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName", assignment.SchoolId);
                        ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x => x.DepartmentName), "DepartmentId", "FullDepartment", assignment.DepartmentId);
                        ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x => x.LevelName), "LevelId", "FullLevel", assignment.LevelId);
                        return View(assignment);
                    }
                    else if (file.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 3; //3 MB size

                        string[] AllowdFileExtensions = new string[] { ".pdf", ".doc", "docx", ".xls", ".xlsx","pub" };

                        if (!AllowdFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf("."))))
                        {
                            ModelState.AddModelError("File", "Please file of type:" + string.Join(",", AllowdFileExtensions));
                            ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName", assignment.SchoolId);
                            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x => x.DepartmentName), "DepartmentId", "FullDepartment", assignment.DepartmentId);
                            ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x => x.LevelName), "LevelId", "FullLevel", assignment.LevelId);
                            return View(assignment);
                        }
                        else if (file.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", " Your file is too large, maximum allowed size is " + MaxContentLength + " MB ");
                            ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName", assignment.SchoolId);
                            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x => x.DepartmentName), "DepartmentId", "FullDepartment", assignment.DepartmentId);
                            ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x => x.LevelName), "LevelId", "FullLevel", assignment.LevelId);
                            return View(assignment);
                        }
                        else
                        {

                            string fname = Path.Combine(Server.MapPath("~/Uploads/Assignments/"), Path.GetFileName(genNumber + file.FileName));
                            file.SaveAs(fname);
                            assignment.FilePath = fname;
                            ModelState.Clear();
                            TempData["Error"] = "File uploaded successfully";

                            string textHtml = HttpUtility.HtmlDecode(assignment.Content);
                            textHtml = Regex.Replace(textHtml, @"<DIV>", "<P>", RegexOptions.IgnoreCase);
                            textHtml = Regex.Replace(textHtml, @"</DIV>", "</P>", RegexOptions.IgnoreCase);
                            assignment.Content = textHtml;
                        }
                    }
            

                    assignment.UserId = userdetails.UserId;
                    assignment.Date = DateTime.Now;
                    
                    db.Assignments.Add(assignment);
                    db.SaveChanges();
                    return RedirectToAction("ControlPanel", "Profile");
                }
                catch (Exception e)
                {
                    TempData["Error"] = e.Message;
                    ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName", assignment.SchoolId);
                    ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x => x.DepartmentName), "DepartmentId", "FullDepartment", assignment.DepartmentId);
                    ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x => x.LevelName), "LevelId", "FullLevel", assignment.LevelId);
                    return View(assignment);
                }
            }

            ViewBag.SchoolId = new SelectList(db.Schools.OrderBy(x => x.SchoolName), "SchoolId", "SchoolName",assignment.SchoolId);
            ViewBag.DepartmentId = new SelectList(db.Departments.OrderBy(x => x.DepartmentName), "DepartmentId", "FullDepartment", assignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels.OrderBy(x => x.LevelName), "LevelId", "FullLevel", assignment.LevelId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Assignment assignment = db.Assignments.Find(id);
            if (assignment == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", assignment.DepartmentId);
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "FullLevel", assignment.LevelId);
            return View(assignment);
        }

        //
        // POST: /Assignment/Edit/5

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
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "FullLevel", assignment.LevelId);
            return View(assignment);
        }

        //
        // GET: /Assignment/Delete/5

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
        // POST: /Assignment/Delete/5

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

        //--------------------Casecading Dropdown-----------------------
        public JsonResult DeptList(int id)
        {
            var local = from s in db.Departments.OrderBy(p => p.DepartmentName).Include(x => x.School)
                        where s.SchoolId == id
                        select s;

            return Json(new SelectList(local.ToList(), "DepartmentId", "DepartmentName"), JsonRequestBehavior.AllowGet);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using PagedList;
using System.IO;
using System.Text.RegularExpressions;


namespace MicroAssignment.Areas.MicroAdmin.Controllers
{
    public class SchoolMaterialController : Controller
    {
        private MicroContext db = new MicroContext();
        System.Random randomInteger = new System.Random();
        //
        // GET: /MicroAdmin/SchoolMaterial/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
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

            var materials = from s in db.Materials.Include(m => m.Department)
                              select s;

        
           if (!String.IsNullOrEmpty(searchString))
            {
                materials = materials.Where(s => s.Topic.ToUpper().Contains(searchString.ToUpper())
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
        // GET: /MicroAdmin/SchoolMaterial/Details/5

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
        // GET: /MicroAdmin/SchoolMaterial/Create

        public ActionResult Create()
        {
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName");
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName");
            return View();
        }

        //
        // POST: /MicroAdmin/SchoolMaterial/Create

        [HttpPost]
        public ActionResult Create(Material material, HttpPostedFileBase file)
        {
            int genNumber = randomInteger.Next(1234567,1234567);
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
            if (ModelState.IsValid)
            {
                try
                {
                    if (file == null)
                    {
                        ModelState.AddModelError("File", "Please upload file");
                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
                        return View(material);
                    }
                    else if (file.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 3; //3 MB size

                        string[] AllowdFileExtensions = new string[] { ".pdf", ".doc", "docx" };

                        if (!AllowdFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf("."))))
                        {
                            ModelState.AddModelError("File", "Please file of type:" + string.Join(",", AllowdFileExtensions));
                        }
                        else if (file.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", " Your file is too large, maximum allowed size is " + MaxContentLength + " MB ");
                        }
                        else
                        {

                            string fname = Path.Combine(Server.MapPath("~/Uploads/Materials/"), Path.GetFileName(file.FileName));
                            file.SaveAs(fname);
                            material.DocumentUrl = fname;
                            ModelState.Clear();
                            ViewBag.Message = "File uploaded successfully";
                            string textHtml = HttpUtility.HtmlDecode(material.Description);
                            textHtml = Regex.Replace(textHtml, @"<DIV>","<P>", RegexOptions.IgnoreCase);
                            textHtml = Regex.Replace(textHtml, @"</DIV>", "</P>", RegexOptions.IgnoreCase);
                            material.Description = textHtml;
                        }
                        material.UserId = userDetails.UserId;
                        material.DateUploaded = DateTime.Now;
                        db.Materials.Add(material);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                }
                catch
                {
                    TempData["error"] = "Invalid format! please try another image";
                    ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
                    return View(material);
                }
               
                
            }
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "DepartmentName", material.DepartmentId);
            return View(material);
        }

        //
        // GET: /MicroAdmin/SchoolMaterial/Edit/5

        public ActionResult Edit(int id = 0)
        {
            Material material = db.Materials.Find(id);
            if (material == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", material.UserId);
            return View(material);
        }

        //
        // POST: /MicroAdmin/SchoolMaterial/Edit/5

        [HttpPost]
        public ActionResult Edit(Material material)
        {
            if (ModelState.IsValid)
            {
                db.Entry(material).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserId = new SelectList(db.UserProfiles, "UserId", "UserName", material.UserId);
            return View(material);
        }

        //
        // GET: /MicroAdmin/SchoolMaterial/Delete/5

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
        // POST: /MicroAdmin/SchoolMaterial/Delete/5

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
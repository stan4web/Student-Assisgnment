using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using System.Text.RegularExpressions;
using System.IO;

namespace MicroAssignment.Controllers
{
    public class ReplyController : Controller
    {
        private MicroContext db = new MicroContext();
        System.Random randomInteger = new System.Random();
        //
        // GET: /Reply/

        public ActionResult Index(int id)
        {
            ViewBag.AssignmentId = id;
            var assignment = db.Assignments.FirstOrDefault(x => x.AssignmentId == id);
            ViewBag.Assignment = assignment.CourseName+", Topic: "+assignment.Topic;
            var replyassignments = db.ReplyAssignments.Where(x=>x.AssignmentId==id).Include(r => r.Assignment).Include(r => r.Department).Include(r => r.Levels).Include(r => r.UserProfile);
            return View(replyassignments.ToList());
        }

        //
        // GET: /Reply/Details/5

        public ActionResult Details(int id = 0)
        {
            var reply = db.ReplyAssignments.Include(x => x.Assignment).Include(x => x.UserProfile).Include(x=>x.Department).Include(x=>x.Levels);
            var replyassignment = reply.FirstOrDefault(x => x.ReplyAssignmentId == id);
            if (replyassignment == null)
            {
                return HttpNotFound();
            }
            return View(replyassignment);
        }

        [HttpPost]
        public ActionResult UpdateScore(int id, string Score, int assignmentId)
        {
            try
            {
                int stdScore = int.Parse(Score);
                if (stdScore > 40)
                {
                    TempData["error"] = "Sorry! the most be the range of 0-40";
                    return RedirectToAction("AddScore", new { id = id, assignmentId = assignmentId });
                }
                var reply = db.ReplyAssignments.FirstOrDefault(x => x.ReplyAssignmentId == id);
                reply.Score = stdScore;
                db.SaveChanges();
                return RedirectToAction("Index", new { id = assignmentId });
            }
            catch (Exception e)
            {
                TempData["Error"] = e.Message;
                return RedirectToAction("AddScore", new { id = id, assignmentId = assignmentId });
            }   
        }

        //
        // GET: /Reply/Create

        public ActionResult Create(int id, int departmentId, int levelId, int lecturerId)
        {
            var alldata=db.Assignments.Include(c=>c.UserProfile);
            var assignment = alldata.FirstOrDefault(x => x.AssignmentId == id);

            ViewBag.Topic = assignment.Topic;
            ViewBag.Lecturer = assignment.UserProfile.FullName;
            ViewBag.Content = assignment.Content;
            ViewBag.Date = assignment.ClosingDate.Value.Date;
            ViewBag.Date2 = assignment.Date.Value.Date;



            ViewBag.lecturerId = assignment.UserId;
            ViewBag.levelId = assignment.LevelId;
            ViewBag.DepartmentId = departmentId;

            return View();
        }

        //
        // POST: /Reply/Create

        [HttpPost]
        public ActionResult Create(ReplyAssignment replyassignment, int id, HttpPostedFileBase file)
        {
            var userDetails = db.UserProfiles.FirstOrDefault(p => p.UserName == User.Identity.Name);
            var verify = db.ReplyAssignments.FirstOrDefault(x => x.AssignmentId == id && x.StudentId==userDetails.UserId);
            int genNumber = randomInteger.Next(1234567890);


            var alldata = db.Assignments.Include(c => c.UserProfile);
            var assignment = alldata.FirstOrDefault(x => x.AssignmentId == id);

            ViewBag.Topic = assignment.Topic;
            ViewBag.Lecturer = assignment.UserProfile.FullName;
            ViewBag.Content = assignment.Content;
            ViewBag.Date = assignment.ClosingDate.Value.Date;
            ViewBag.Date2 = assignment.Date.Value.Date;

            ViewBag.lecturerId = assignment.UserId;
            ViewBag.levelId = assignment.LevelId;

            ViewBag.DepartmentId = replyassignment.DepartmentId;
            if (ModelState.IsValid)
            {
                if (verify != null)
                {
                    TempData["error"] = "Sorry! You can not submit more than once.";
                    return View(replyassignment);
                }

                try
                {
                    if (file == null)
                    {
                        ModelState.AddModelError("File", "Please upload file");
                        TempData["Error"] = "Please upload file";
                       
                        return View(replyassignment);
                    }
                    else if (file.ContentLength > 0)
                    {
                        int MaxContentLength = 1024 * 1024 * 3; //3 MB size

                        string[] AllowdFileExtensions = new string[] { ".pdf", ".doc", "docx", ".xls", ".xlsx", "pub" };

                        if (!AllowdFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf("."))))
                        {
                            ModelState.AddModelError("File", "Please file of type:" + string.Join(",", AllowdFileExtensions));
                         
                            return View(replyassignment);
                        }
                        else if (file.ContentLength > MaxContentLength)
                        {
                            ModelState.AddModelError("File", " Your file is too large, maximum allowed size is " + MaxContentLength + " MB ");

                            return View(replyassignment);
                        }
                        else
                        {

                            string fname = Path.Combine(Server.MapPath("~/Uploads/SumittedAssignments/"), Path.GetFileName(genNumber + file.FileName));
                            file.SaveAs(fname);
                            replyassignment.FilePath = fname;
                            ModelState.Clear();
                            TempData["Error"] = "File uploaded successfully";

                            string textHtml = HttpUtility.HtmlDecode(replyassignment.Content);
                            textHtml = Regex.Replace(textHtml, @"<DIV>", "<P>", RegexOptions.IgnoreCase);
                            textHtml = Regex.Replace(textHtml, @"</DIV>", "</P>", RegexOptions.IgnoreCase);
                            replyassignment.Content = textHtml;
                        }
                    }
                }
                catch (Exception e)
                {
                    TempData["Error"] = e.Message;
                    return View(replyassignment);
                }

                replyassignment.AssignmentId = id;
                replyassignment.DepartmentId = replyassignment.DepartmentId;
                replyassignment.LevelId = replyassignment.LevelId;
                replyassignment.UserId = replyassignment.UserId;
                replyassignment.Date = DateTime.Now;
                replyassignment.StudentId = userDetails.UserId;

                db.ReplyAssignments.Add(replyassignment);
                db.SaveChanges();

                return RedirectToAction("Index","profile");
            }

            return View(replyassignment);
        }

        //
        // GET: /Reply/Edit/5

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
        // POST: /Reply/Edit/5

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
        // GET: /Reply/Delete/5

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
        // POST: /Reply/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ReplyAssignment replyassignment = db.ReplyAssignments.Find(id);
            db.ReplyAssignments.Remove(replyassignment);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult AddScore(int id, int assignmentId)
        {
            ViewBag.AssignmentId = assignmentId;
            var rplay = db.ReplyAssignments.Include(x => x.Assignment).Include(z => z.UserProfile);
            var replyassignment = db.ReplyAssignments.FirstOrDefault(x=>x.ReplyAssignmentId==id);

            if (replyassignment == null)
            {
                return HttpNotFound();
            }
            return View(replyassignment);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public ActionResult Result(int id, int TestScore, int ExamScore, int assignmentId)
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
            var department = db.Departments.FirstOrDefault(z => z.DepartmentId == userDetails.DepartmentId);
            var school = db.Schools.FirstOrDefault(s => s.SchoolId == department.SchoolId);
            
            var reply = db.ReplyAssignments.FirstOrDefault(x => x.ReplyAssignmentId == id);
            reply.TestScore = TestScore;
            reply.ExamScore = ExamScore;

            int totalresult = TestScore + ExamScore + reply.Score.Value;

            if (school.IsUniversity == false)
            {
                //polytechnic grading system here.
                if (totalresult >= 80)
                {
                    reply.Avrage = "A";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 70)
                {
                    reply.Avrage = "AB";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 60)
                {
                    reply.Avrage = "B";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 50)
                {
                    reply.Avrage = "BC";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 40)
                {
                    reply.Avrage = "C";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 30)
                {
                    reply.Avrage = "CD";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult >= 20)
                {
                    reply.Avrage = "D";
                    reply.TotalSCore = totalresult;
                }
                else if (totalresult < 20)
                {
                    reply.Avrage = "F";
                    reply.TotalSCore = totalresult;
                }
            }
            else if (school.IsUniversity == true)
            {
                //university grading system here...
            }

            db.SaveChanges();
            return RedirectToAction("Index", "Reply", new { id = assignmentId });
        }
    }
}
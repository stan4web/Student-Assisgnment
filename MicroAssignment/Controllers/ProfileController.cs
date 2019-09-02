using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using MicroAssignment.Models;
using System.Data;
using System.Web.Helpers;
using System.IO;
using PagedList;

namespace MicroAssignment.Controllers
{
     [Authorize]
    public class ProfileController : Controller
    {
        private MicroContext db = new MicroContext();
        System.Random randomInteger = new System.Random();
        //
        // GET: /Profile/

#region User profile
        // complate registration
         [HttpGet]
        public ActionResult ComplateRegistration(int id)
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserId == id);
            UserProfile user = db.UserProfiles.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            //var departmet = db.Departments.Where(x => x.SchoolId == userDetails.SchoolId).OrderBy(z=>z.DepartmentName);
            //ViewBag.DepartmentId =new SelectList(departmet, "DepartmentId", "DepartmentName");

            var level = db.Levels.OrderBy(x => x.LevelName);
            ViewBag.LevelId = new SelectList(level, "LevelId", "FullLevel");
            return View(user);
        }

         [HttpPost]
         public ActionResult ComplateRegistration(UserProfile profile)
         {
             var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserId == profile.UserId);
             //var departmet = db.Departments.Where(x => x.SchoolId == userDetails.SchoolId).OrderBy(z => z.DepartmentName);
             var level = db.Levels.OrderBy(x => x.LevelName);
             var userAccount = db.UserProfiles.FirstOrDefault(c => c.UserName == User.Identity.Name);
             if (ModelState.IsValid)
             {
                 try
                 {
                     
                     userAccount.IsComplated = true;
                     userAccount.SurName = profile.SurName.ToUpper();
                     userAccount.FirstName = profile.FirstName.ToUpper();
                     if (profile.OtherNames != null)
                     {
                         userAccount.OtherNames = profile.OtherNames.ToUpper();
                     }
                     userAccount.RegistrationNumber = profile.RegistrationNumber.ToUpper();
                     userAccount.DepartmentId = profile.DepartmentId;
                     userAccount.LevelId = profile.LevelId;

                     db.SaveChanges();
                     return RedirectToAction("Index");
                 }
                 catch(Exception e)
                 {
                     TempData["Error"] = e.Message;
                     //ViewBag.DepartmentId = new SelectList(departmet, "DepartmentId", "DepartmentName",profile.DepartmentId);
                     ViewBag.LevelId = new SelectList(level, "LevelId", "FullLevel", profile.LevelId);
                     return View(profile);
                 }
             }
             return View(profile);
         }


         public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
            {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "PostId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "PostId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            //advertisement
            var adds = db.Advertising.ToArray();
            ViewBag.Advert = adds; 

            ViewBag.Profile = "active-menu";
            
            var userDetails = db.UserProfiles.Include(c => c.Department).Include(c => c.Level);

            var profile = userDetails.FirstOrDefault(x => x.UserName == User.Identity.Name);
            
            UserProfile user = db.UserProfiles.Find(profile.UserId);

            ViewBag.FullName = user.SurName.Substring(0, 1).ToUpper() + user.SurName.Substring(1).ToLower() + " " + user.FirstName.Substring(0, 1).ToUpper() + user.FirstName.Substring(1).ToLower();
            ViewBag.UserId = user.UserId;
            ViewBag.DeparmentId = user.DepartmentId;
            ViewBag.Department = user.Department.DepartmentName;
            ViewBag.RegNo = user.RegistrationNumber;
            if (user == null)
            {
                return HttpNotFound();
            }

           
            if (User.IsInRole("Instructor"))
            {
                return RedirectToAction("ControlPanel");
            }
           
            else if (User.IsInRole("SuperAdmin"))
            {
                return RedirectToAction("Index", "Home", new { area = "MicroAdmin" });
            }

            var post = from s in db.Posts.Where(z => z.DepartmentId == user.DepartmentId).OrderByDescending(x => x.PostId).Include(z => z.Comments).Include(p => p.UserProfile).ToList()
                                select s;


            if (!String.IsNullOrEmpty(searchString))
            {
                post = post.Where(s => s.PostContent.ToUpper().Contains(searchString.ToUpper())
                    || s.PostDate.ToString().Contains(searchString.ToUpper())
                    || s.UserProfile.SurName.ToString().Contains(searchString.ToUpper())
                    || s.UserProfile.FirstName.ToString().Contains(searchString.ToUpper())
                    || s.UserProfile.EmailAddress.ToString().Contains(searchString.ToUpper())
                    || s.UserProfile.PhoneNumber.ToString().Contains(searchString.ToUpper())
                    || s.UserProfile.UserName.Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "PostId_desc":
                    post = post.OrderByDescending(x => x.PostId);
                    break;
                default:
                    post = post.OrderBy(x => x.PostId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(post.ToPagedList(pageNumber, pageSize));
        }

        //
        // POST: /Profile/Edit/5

        [HttpPost]
        public ActionResult EditProfile(UserProfile userprofile)
        {
            int genNumber = randomInteger.Next(1234567890);
            if (ModelState.IsValid)
            {
                try
                {
                    if (Request.Files.Count > 0)
                    {
                        HttpPostedFileBase file = Request.Files[0];
                        if (file.ContentLength > 0 && file.ContentType.ToUpper().Contains("JPEG") || file.ContentType.ToUpper().Contains("PNG"))
                        {

                            WebImage img = new WebImage(file.InputStream);
                            if (img.Width > 200)
                            {
                                img.Resize(200, 200, true, true);
                            }
                            string fileName = Path.Combine(Server.MapPath("~/Uploads/Items/"), Path.GetFileName(genNumber + file.FileName));
                            img.Save(fileName);
                            userprofile.ImageUrl = fileName;
                        }

                    }
                    db.Entry(userprofile).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch
                {

                    ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "FullLevel", userprofile.LevelId);
                    return View(userprofile);
                }
            }
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "FullLevel",userprofile.LevelId);
            return View(userprofile);
        }

        public ActionResult ControlPanel()
        {
            ViewBag.Profile = "active-menu";
            var userAccount = db.UserProfiles.Include(x => x.Department);
            var userDetails = userAccount.FirstOrDefault(x => x.UserName == User.Identity.Name);
            ViewBag.Dept = userDetails.Department.DepartmentName;

            ViewBag.ProfileName = userDetails.SurName.Substring(0, 1).ToUpper() + userDetails.SurName.Substring(1).ToLower() + " " + userDetails.FirstName.Substring(0, 1).ToUpper() + userDetails.FirstName.Substring(1).ToLower();

            if (userDetails == null)
            {
                return HttpNotFound();
            }

            ViewBag.Department = db.Departments.OrderBy(x=>x.DepartmentName).ToArray();

            var reply = db.Assignments.Where(x => x.UserId == userDetails.UserId).Include(x => x.UserProfile).Include(x => x.Department).Include(x => x.Levels);
            ViewBag.Assignment = reply.OrderByDescending(X=>X.AssignmentId).ToArray();

            return View(userDetails);
        }

        public ActionResult ControlMenu(int id)
        {
           
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);

            if (userDetails == null)
            {
                return HttpNotFound();
            }
            var department = db.Departments.FirstOrDefault(x => x.DepartmentId == id);
            ViewBag.Department = department.FullDepartment;
            ViewBag.DepartmentId = department.DepartmentId;

            ViewBag.Level = db.Levels.OrderBy(x => x.LevelName).ToArray();
            return View(userDetails);
        }

        public ActionResult ControlLevel(int id, int departmentId, int levelId, int userId)
        {
            ViewBag.Id = id;
            var students = db.ReplyAssignments.Where(x => x.DepartmentId == departmentId && x.UserId == userId && x.LevelId == levelId).Include(x => x.Department).Include(s => s.UserProfile).ToList();
            var department = db.Departments.FirstOrDefault(x => x.DepartmentId == departmentId);
            var level = db.Levels.FirstOrDefault(x => x.LevelId == levelId);
            ViewBag.Deparment = department.FullDepartment;
            ViewBag.Level = level.FullLevel;

            return View(students);
        }

#endregion

#region Assignment
        public ActionResult _Assignment()
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);


            var assignment = db.Assignments.Where(x => x.DepartmentId == userDetails.DepartmentId && x.LevelId == userDetails.LevelId).Include(x => x.Department).Include(z => z.Levels).Include(a => a.UserProfile).ToArray();
            ViewBag.Assignment = assignment;

            return PartialView();
        }


        public ActionResult _SubmitAssignment()
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);

            var submitAssignment = db.ReplyAssignments.Where(x => x.StudentId == userDetails.UserId).Include(z => z.Assignment).Include(x => x.Department).Include(z => z.Levels).Include(z => z.UserProfile).ToArray();
            ViewBag.SubmitAssignment = submitAssignment.Take(5);

            return PartialView();
        }


        public ActionResult _ScoredAssignment()
        {
            var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);

            var submitAssignment = db.ReplyAssignments.Where(x => x.StudentId == userDetails.UserId && x.Score != null).Include(z => z.Assignment).Include(x => x.Department).Include(z => z.Levels).Include(z => z.UserProfile).ToArray();
            ViewBag.SubmitedAssignment = submitAssignment;

            return PartialView();
        }

        // GET: /Profile/Edit/5

        public ActionResult EditProfile(int id = 0)
        {
            var userDetails = db.UserProfiles.Include(x => x.Department).Include(z => z.Level);
            var userprofile = userDetails.FirstOrDefault(p => p.UserId == id);
            if (userprofile == null)
            {
                return HttpNotFound();
            }
            ViewBag.LevelId = new SelectList(db.Levels, "LevelId", "FullLevel", userprofile.LevelId);
            return View(userprofile);
        }

        // GET: /Department/Details/5

        public ActionResult AssignmentDetails(int id = 0)
        {
            var assignment = db.Assignments.Include(x => x.UserProfile);
            var assign = assignment.Single(z => z.AssignmentId == id);
            if (assign == null)
            {
                return HttpNotFound();
            }
            return View(assign);
        }

        public ActionResult SubmitedAssignment(int id, int departmentId, int levelId)
        {
            var assignment = db.Assignments.Where(x => x.UserId == id && x.DepartmentId == departmentId && x.LevelId == levelId ).ToList();
            ViewBag.UserId = id;
            ViewBag.DepartmentId = departmentId;
            ViewBag.LevelId = levelId;

            return View(assignment);
        }
#endregion

#region Social media
        [HttpPost]
        public ActionResult Post(int id, string Content, int DepartmentId, string IsPublic)
            {
                if (Content=="")
                {
                    TempData["validation"] = "Validation Error! Please type your post on the textbox provided bellow.";
                    return RedirectToAction("Index");
                }
                Post post = db.Posts.Create();
                post.UserId = id;
                post.DepartmentId = DepartmentId;
                post.PostContent = Content;
                post.PostDate = DateTime.Now;
                if (IsPublic == "Yes")
                {
                    post.IsPublic = true;
                }
                else if(IsPublic == "No")
                {
                    post.IsPublic = false;
                }
                db.Posts.Add(post);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        public ActionResult _PostList()
            {
                var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
                var post = db.Posts.Where(x => x.UserId == userDetails.UserId).OrderByDescending(z=>z.PostId).Include(x=>x.UserProfile).Include(x=>x.Comments);
                ViewBag.Post = post.ToArray();
                ViewBag.Name = userDetails.SurName;
                return PartialView();
            }

        public ActionResult Forum(string searchString, string currentFilter, int? page, string sortOrder)
            {
                var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
                ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "PostId desc" : "";
                ViewBag.CurrentSort = sortOrder;
                ViewBag.Forum = "active-menu";

                var post = from s in db.Posts.Where(c=>c.IsPublic==true || c.DepartmentId==userDetails.DepartmentId).Include(x=>x.UserProfile).Include(z=>z.Comments)
                          select s;

                if (!string.IsNullOrEmpty(searchString))
            {
                post = post.Where(s => s.UserProfile.UserName.ToUpper().Contains(searchString.ToUpper())
               || s.UserProfile.SurName.ToUpper().Contains(searchString.ToUpper())
               || s.UserProfile.FirstName.ToUpper().Contains(searchString.ToUpper())
               || s.PostContent.ToUpper().Contains(searchString.ToUpper()));
            }

            switch (sortOrder)
            {
                case "PostId desc":
                    post = post.OrderByDescending(s => s.PostId);
                    break;


                default:
                    post = post.OrderByDescending(s => s.PostId);
                    break;
            }


            if (Request.HttpMethod == "GET")
            {
                searchString = currentFilter;
            }
            else
            {
                page = 1;
            }

            ViewBag.CurrentFilter = searchString;


            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(post.ToPagedList(pageNumber, pageSize));
        
            }

        [HttpPost]
        public ActionResult PostComment(int id, string Content)
            {
                var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
                Comment comment = db.Comments.Create();
                comment.PostId = id;
                comment.UserId = userDetails.UserId;
                comment.Content = Content;
                comment.CommentDate = DateTime.Now;
                db.Comments.Add(comment);
                db.SaveChanges();

                return RedirectToAction("Index");
            }

        public ActionResult ResultSheet(int id, string courseCode)
            {
                ViewBag.Code = courseCode;
                var reply = db.ReplyAssignments.Where(x => x.AssignmentId == id).OrderByDescending(x=>x.UserProfile.RegistrationNumber).Include(x=>x.UserProfile).Include(c=>c.Assignment).ToList();
                var assignment = db.Assignments.FirstOrDefault(x => x.AssignmentId == id);
                ViewBag.Assignment = assignment.Topic;
                ViewBag.Name = assignment.CourseName;
                return View(reply);
            }

        public ActionResult ListAssignment()
            {
                ViewBag.Assignment = "active-menu";
                return View();
            }

        public ActionResult ListReplied()
            {
                ViewBag.Replies = "active-menu";
                return View();
            }

        public ActionResult _AdvertList()
            {
                var adds = db.Advertising.Where(z=>z.IsEnabled==true).OrderBy(x => Guid.NewGuid()).ToArray();
                ViewBag.Advert = adds;
                return PartialView();
            }
#endregion

            //students enrollmet
#region Enrollment
            [HttpGet]
            public ActionResult Enrollment(int id)
            {
                ViewBag.CourseRegistration = "active-menu";
                var userDetails = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);

                ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession");
                var department = db.Departments.FirstOrDefault(x => x.DepartmentId == userDetails.DepartmentId);

                var enrolledSession = db.Enrollments.Include(x => x.Session).Include(z => z.Department).Include(a => a.UserProfile);

                var enrolledstudent = enrolledSession.FirstOrDefault(x => x.DepartmentId == userDetails.DepartmentId && x.UserId == userDetails.UserId && x.Session.IsCurrent == true);

                if (enrolledstudent != null)
                {
                    var session = db.Sessions.FirstOrDefault(x => x.SessionId == enrolledstudent.SessionId);
                    ViewBag.sessName = session.SessionId;
                    ViewBag.CurrentYear = enrolledstudent.Session.FullSession;
                }

                ViewBag.enrollment = enrolledstudent;
                

                ViewBag.id = id;
                if (enrolledstudent != null)
                {
                    ViewBag.enrollmentId = enrolledstudent.EnrollmentId;
                }
                ViewBag.dept = id;
                ViewBag.deptId = department.DepartmentId;
                return View();
            }


            [HttpPost]
            public ActionResult Enrollment(Enrollment enrollment, int id)
            {
                ViewBag.CourseRegistration = "active-menu";
                try
                {
                    var department = db.Departments.FirstOrDefault(p => p.DepartmentId == id);
                    var user = db.UserProfiles.FirstOrDefault(x => x.UserName == User.Identity.Name);
                    var sess = db.Sessions.FirstOrDefault(p => p.SessionId == enrollment.SessionId);
                    if (department == null)
                    {
                        Session["error"] = "Sorry you do not have Department";
                        return RedirectToAction("Index", "MyProfile");
                    }
                    ViewBag.deptId = department.DepartmentId;
                    if (ModelState.IsValid)
                    {
                        var enrolledstudent = db.Enrollments.OrderByDescending(x => x.EnrollmentId).FirstOrDefault(x => x.UserId == user.UserId && x.EnrollmentId == id);
                        if (enrolledstudent != null)
                        {
                            Session["error"] = "Dear" + " " + user.FullName + " " + "You are currently enrolled. ";
                            ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", enrollment.EnrollmentId);
                            return View(enrollment);
                        }
                        else
                        {

                            enrollment.DepartmentId = department.DepartmentId;
                            enrollment.UserId = user.UserId;

                            db.Enrollments.Add(enrollment);
                            db.SaveChanges();
                            Session["success"] = "Congratulation! Your enrollment has been successfully created, Proceed to course regisgration";
                            return RedirectToAction("Enrollment", new { id = id, sessId = sess.SessionId });
                        }
                    }
                    ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", enrollment.EnrollmentId);
                }
                catch (Exception e)
                {
                    TempData["error"] = e.Message;
                    ViewBag.SessionId = new SelectList(db.Sessions, "SessionId", "FullSession", enrollment.EnrollmentId);
                    return View(enrollment);
                }
                return View(enrollment);
            }

#endregion

            //Course registration
#region Course registration
            public ActionResult SaveCourses(int id, int sessId)
            {
                ViewBag.CourseRegistration = "active-menu";
                var enrollment = db.Enrollments.FirstOrDefault(x => x.EnrollmentId == id);
                var courses = db.Course.Where(x => x.DepartmentID == enrollment.DepartmentId && x.SessionId == sessId);
                var enrolledcourses = db.EnrolledCourses.FirstOrDefault(p => p.EnrollmentId == id);

                //Check if enrolled courses has been created?
                if (enrolledcourses == null)
                {
                    try
                    {
                        foreach (var item in courses)
                        {
                            EnrolledCourse enrolledCourse = db.EnrolledCourses.Create();
                            enrolledCourse.CourseID = item.CourseID;
                            enrolledCourse.EnrollmentId = enrollment.EnrollmentId;
                            enrolledCourse.Grade = "";
                            enrolledCourse.Remark = "Not Entered";
                            enrolledCourse.TestScore = 0;
                            enrolledCourse.ExamScore = 0;
                            enrolledCourse.TotalScore = 0;
                            db.EnrolledCourses.Add(enrolledCourse);
                        }
                        db.SaveChanges();
                    }
                    catch
                    {
                        throw;
                    }
                }
                return RedirectToAction("CourseRegistration", new { id = id });
            }

            //[HttpGet]
            public ActionResult CourseRegistration(int id)
            {
                ViewBag.CourseRegistration = "active-menu";
                var enrollment = db.Enrollments.FirstOrDefault(x => x.EnrollmentId == id);
                var enrolledCourse = db.EnrolledCourses.Include(p => p.Course).Where(x => x.EnrollmentId == id).ToList();
                var userDetails = db.UserProfiles.FirstOrDefault(z => z.UserId == enrollment.UserId);
                var departmentId = db.Departments.FirstOrDefault(y => y.DepartmentId == enrollment.DepartmentId);
                ViewBag.enrolledCourse = enrolledCourse;

                ViewBag.TotalUnit = enrolledCourse.Sum(x => x.Course.CreditUnit);

                var carryover = enrolledCourse.Where(a => a.IsCarryover == true);
                int total = 0;
                foreach (var item in carryover)
                {
                    total = total + item.Course.CreditUnit;
                }

                ViewBag.enrollmentId = enrollment.EnrollmentId;
                ViewBag.deptId = departmentId.DepartmentId;
                ViewBag.dept = departmentId.DepartmentName;
                ViewBag.imageUrl = userDetails.ImageUrl;
                ViewBag.submit = enrollment.IsSubmitted;
                ViewBag.approve = enrollment.IsApproved;
                ViewBag.overall = total + ViewBag.TotalUnit;
                return View(enrolledCourse);
            }
#endregion

            //Carry over courses
#region Carry over courses
            public ActionResult _CarryOverList(int id)
            {

                ViewBag.enrollmentId = id;
                var courses = db.CarryOvers.Where(p => p.EnrollmentId == id).OrderByDescending(x => x.CarryOverId).Include(x => x.Course).Include(v=>v.Enrollment);
                ViewBag.carryover = courses.ToArray();
                if (courses.Count() > 0)
                {
                    ViewBag.TotalUnit = courses.Sum(x => x.Course.CreditUnit);
                }
                else
                {
                    ViewBag.Empty = "True";
                }
               
                return PartialView();
            }

            public ActionResult _CarryOverList2(int id, int deptId)
            {
                ViewBag.deptId = deptId;
                ViewBag.enrollmentId = id;
                //var courses = db.CarryOvers.Where(p => p.EnrollmentId == id).ToList();
                //ViewBag.carryover = courses;
                ViewBag.dept = db.Departments.FirstOrDefault(x => x.DepartmentId == deptId).DepartmentName;
                //ViewBag.TotalUnit = courses.Sum(x => x.CreditUnit);

                return PartialView();
            }

            [HttpGet]
            public ActionResult AddCarryOver(int id)
            {
                ViewBag.CourseRegistration = "active-menu";
                ViewBag.errolmentId = id;
                var courseList = db.Course.OrderBy(p => p.CourseName);
                ViewBag.CourseID = new SelectList(courseList, "CourseID", "CourseName");
                return View();
            }

            [HttpPost]
            public ActionResult AddCarryOver(CarryOver carryover, int id)
            {
                ViewBag.CourseRegistration = "active-menu";
                //Enrollment enrolment = db.Enrollments.Find(id);
                var enrolment = db.Enrollments.FirstOrDefault(x => x.EnrollmentId == id);
                var stdData = db.Sessions.Where(x => x.SessionId == enrolment.SessionId);
                var session = stdData.FirstOrDefault().FullSession;

                var checkcourses = db.CarryOvers.FirstOrDefault(p => p.CourseID == carryover.CourseID && p.EnrollmentId == id);
                //var courses = db.Courses.FirstOrDefault(p => p.CourseName == carryover.CourseName);

                if (ModelState.IsValid)
                {
                    if (checkcourses != null)
                    {
                        TempData["error"] = "Course already exist";
                        ViewBag.CourseID = new SelectList(db.Course, "CourseID", "CourseName", carryover.CourseID);
                        return View(carryover);
                    }
                    else
                    {
                        carryover.EnrollmentId = enrolment.EnrollmentId;

                        carryover.TestScore = 0;
                        carryover.ExamScore = 0;
                        carryover.TotalScore = 0;
                        carryover.Grade = "";
                        carryover.Remark = "Not Entered";

                        carryover.IsCarryover = true;

                        db.CarryOvers.Add(carryover);
                        db.SaveChanges();

                        return RedirectToAction("AddCarryOver", new { id = id });
                    }
                }
                ViewBag.CourseID = new SelectList(db.Course, "CourseID", "CourseName", carryover.CourseID);
                return View(carryover);
            }
#endregion

            //Delete carry over courses
#region Delete carry over course

            public ActionResult DeleteCarryOver(int enrollmentId, int id = 0)
            {
                ViewBag.CourseRegistration = "active-menu";
                CarryOver carryover = db.CarryOvers.Find(id);
                if (carryover == null)
                {
                    return HttpNotFound();
                }
                return View(carryover);
            }

            //
            // POST: /Management/Level/Delete/5

            [HttpPost, ActionName("DeleteCarryOver")]
            public ActionResult DeleteConfirmed(int enrollmentId, int id)
            {
                CarryOver carryover = db.CarryOvers.Find(id);
                db.CarryOvers.Remove(carryover);
                db.SaveChanges();
                return RedirectToAction("AddCarryOver", new { id = enrollmentId });
            }



            public ActionResult DeleteNormalCourse(int enrollmentId, int id = 0)
            {
                ViewBag.CourseRegistration = "active-menu";
                EnrolledCourse enrolledcourses = db.EnrolledCourses.Find(id);
                if (enrolledcourses == null)
                {
                    return HttpNotFound();
                }
                return View(enrolledcourses);
            }


            [HttpPost, ActionName("DeleteNormalCourse")]
            public ActionResult DeleteConfirmed2(int enrollmentId, int id)
            {
                var course = db.EnrolledCourses.Include(p => p.Course);
                EnrolledCourse enrolledcourses = db.EnrolledCourses.Find(id);
                db.EnrolledCourses.Remove(enrolledcourses);
                db.SaveChanges();
                return RedirectToAction("CourseRegistration", new { id = enrollmentId });
            }
#endregion

#region Submit to course adviser

            [HttpPost]
            public ActionResult SubmitCourse(int id, int deptId, string dept)
            {
                ViewBag.CourseRegistration = "active-menu";
                var enrollment = db.Enrollments.FirstOrDefault(p => p.EnrollmentId == id);
                enrollment.IsSubmitted = true;
                enrollment.IsApproved = true;
                enrollment.IsSaved = true;
                db.SaveChanges();
                Session["submit"] = "Your Course Form Has Been Submitted";
                return RedirectToAction("CoursePreview", new { id = id, deptId = deptId, dept = dept });
            }

            public ActionResult CoursePreview(int id, string dept, int deptId)
            {
                ViewBag.CourseRegistration = "active-menu";
                var enrollment = db.Enrollments.FirstOrDefault(x => x.EnrollmentId == id);
                if (enrollment != null)
                {

                    var enrolledCourse = db.EnrolledCourses.Include(p => p.Course).Where(x => x.EnrollmentId == id).ToList();
                    var userInfo = db.UserProfiles.Include(x => x.Level).Include(c => c.Department);
                    var userDetails = userInfo.FirstOrDefault(z => z.UserId == enrollment.UserId);
                    var departmentId = db.Departments.FirstOrDefault(y => y.DepartmentId == enrollment.DepartmentId);
                    ViewBag.enrolledCourse = enrolledCourse;

                    ViewBag.TotalUnit = enrolledCourse.Sum(x => x.Course.CreditUnit);

                    var carryover = enrolledCourse.Where(a => a.IsCarryover == true);
                    int total = 0;
                    foreach (var item in carryover)
                    {
                        total = total + item.Course.CreditUnit;
                    }

                    ViewBag.enrollmentId = enrollment.EnrollmentId;
                    ViewBag.deptId = departmentId.DepartmentId;
                    ViewBag.dept = departmentId.DepartmentName;

                    ViewBag.imageUrl = userDetails.ImageUrl;
                    ViewBag.matricnumber = userDetails.RegistrationNumber;
                    ViewBag.fullname = userDetails.FullName;
                    ViewBag.level = userDetails.Level.LevelName;

                    ViewBag.submit = enrollment.IsSubmitted;
                    ViewBag.approve = enrollment.IsApproved;
                    ViewBag.overall = total + ViewBag.TotalUnit;
                    return View(enrolledCourse);
                }
                else
                {
                    ViewData["error"] = "Sorry! You are lost";
                    return View();
                }

            }

#endregion
   
     }
}

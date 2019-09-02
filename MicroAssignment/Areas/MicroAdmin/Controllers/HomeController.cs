using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using MicroAssignment.Filters;
using PagedList;
using WebMatrix.WebData;
using System.Data;
using System.Data.Entity;

namespace MicroAssignment.Areas.MicroAdmin.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /MicrowareAdmin/Main/
        private MicroContext db = new MicroContext();
        [Authorize(Roles = "SuperAdmin")]

        public ActionResult Index()
        {
            ViewBag.TotalUser = db.UserProfiles.Count();
            
            return View();
        }


        public ActionResult Users(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "SurName_desc" : "";
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

            var users = from s in db.UserProfiles.Where(x => x.UserId != 1).Include(z=>z.Department).Include(p=>p.Level)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                users = users.Where(s => s.SurName.ToUpper().Contains(searchString.ToUpper())
                                       || s.OtherNames.ToUpper().Contains(searchString.ToUpper())
                                       || s.EmailAddress.ToUpper().Contains(searchString.ToUpper())
                                       || s.PhoneNumber.ToUpper().Contains(searchString.ToUpper())
                                       || s.FirstName.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "SurName_desc":
                    users = users.OrderByDescending(x => x.SurName);
                    break;
                default:
                    users = users.OrderBy(x => x.FirstName);
                    break;

            }




            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(users.ToPagedList(pageNumber, pageSize));
        }


        [InitializeSimpleMembership]
        public ActionResult Roles()
        {
            var roles = System.Web.Security.Roles.GetAllRoles();

            return View(roles);
        }

        [HttpPost]
        public ActionResult AddRole(string rolename)
        {
            System.Web.Security.Roles.CreateRole(rolename);
            var roles = System.Web.Security.Roles.GetAllRoles();

            return View("Roles", roles);
        }
        [HttpPost]
        public ActionResult UserToRole(string rolename, string username, bool? ischecked, int? page)
        {
            if (ischecked.HasValue && ischecked.Value)
            {
                System.Web.Security.Roles.AddUserToRole(username, rolename);
            }
            else
            {
                System.Web.Security.Roles.RemoveUserFromRole(username, rolename);
            }

            return RedirectToAction("Users", new { page = page });
        }

        public ActionResult Post(string sortOrder, string currentFilter, string searchString, int? page)
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

            var post = from s in db.Posts.OrderByDescending(x=>x.PostId)
                        select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                post = post.Where(s => s.PostContent.ToUpper().Contains(searchString.ToUpper()));
            }

            ViewBag.Roles = System.Web.Security.Roles.GetAllRoles();

            switch (sortOrder)
            {
                case "PostId_desc":
                    post = post.OrderByDescending(x => x.PostId);
                    break;
                default:
                    post = post.OrderBy(x => x.UserId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(post.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /post/Delete/5

        public ActionResult DeletePost(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /post/Delete/5

        [HttpPost, ActionName("DeletePost")]
        public ActionResult DeleteConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("Post");
        }

        public ActionResult Comment(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SurnameSortParm = string.IsNullOrEmpty(sortOrder) ? "CommentId_desc" : "";
            ViewBag.DepartmentSortParm = string.IsNullOrEmpty(sortOrder) ? "CommentId_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var comment = from s in db.Comments.OrderByDescending(x => x.CommentId)
                       select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                comment = comment.Where(s => s.Content.ToUpper().Contains(searchString.ToUpper()));
            }

            

            switch (sortOrder)
            {
                case "PostId_desc":
                    comment = comment.OrderByDescending(x => x.CommentId);
                    break;
                default:
                    comment = comment.OrderBy(x => x.CommentId);
                    break;

            }

            int pageSize = 100;
            int pageNumber = (page ?? 1);
            return View(comment.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Comment/Delete/5

        public ActionResult DeleteComment(int id = 0)
        {
            Comment comment = db.Comments.Find(id);
            if (comment == null)
            {
                return HttpNotFound();
            }
            return View(comment);
        }

        //
        // POST: /Comment/Delete/5

        [HttpPost, ActionName("DeleteComment")]
        public ActionResult DeleteConfirmed2(int id)
        {
            Comment comment = db.Comments.Find(id);
            db.Comments.Remove(comment);
            db.SaveChanges();
            return RedirectToAction("Comment");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}

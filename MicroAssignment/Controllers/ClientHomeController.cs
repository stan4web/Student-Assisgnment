using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MicroAssignment.Controllers
{
    public class ClientHomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Home = "active";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.About = "active";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Contact = "active";
            return View();
        }
    }
}

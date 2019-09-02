using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MicroAssignment.Models;
using System.Text.RegularExpressions;

namespace MicroAssignment.Controllers
{
    public class HomePageController : Controller
    {
        private MicroContext db = new MicroContext();
        //
        // GET: /HomePage/

        public ActionResult Index()
        {

            return View();
        }

        public ActionResult _Downloads()
        {
            var material = db.Materials.OrderByDescending(x => x.MaterialId).Include(p=>p.Department);
           
            ViewBag.Download = material.Take(4).ToArray();
           
            return PartialView();
        }

       

    }
}

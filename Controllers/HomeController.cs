using CrowdKnowledge2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AppContext = CrowdKnowledge2.Models.ApplicationDbContext;

namespace CrowdKnowledge2.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new CrowdKnowledge2.Models.ApplicationDbContext();
        public ActionResult Index()
        {

            var articole = db.Articole.Include("Domeniu");

            ViewBag.FirstArticle = articole.First();
            ViewBag.Articles = articole.Where(o => o.IdParent == 0).OrderByDescending(o => o.Data).Take(2);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
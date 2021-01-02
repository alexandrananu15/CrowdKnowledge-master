using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdKnowledge2.Models;

namespace CrowdKnowledge2.Controllers
{

    //Doar Adminul poate face CRUD pe domenii
    public class DomeniiController : Controller
    {
        private CrowdKnowledge2.Models.ApplicationDbContext db = new CrowdKnowledge2.Models.ApplicationDbContext();

        //Afisare lista domenii
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index()
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"].ToString();
            }

            var domenii = from domeniu in db.Domenii
                             orderby domeniu.NumeDomeniu
                             select domeniu;
            ViewBag.Domenii = domenii;
            return View();
        }


        //Afisare domeniu cu id-ul dat
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            Domeniu domeniu = db.Domenii.Find(id);
            return View(domeniu);
        }


        //Adaugare domeniu nou
        [Authorize(Roles = "Admin")]
        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult New(Domeniu dom)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Domenii.Add(dom);
                    db.SaveChanges();
                    TempData["message"] = "Domeniul " + dom.NumeDomeniu +" a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(dom);
                }
            }
            catch (Exception e)
            {
                return View(dom);
            }
        }


        //Editare domeniu cu id-ul dat
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            Domeniu domeniu = db.Domenii.Find(id);
            return View(domeniu);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id, Domeniu requestDomeniu)
        {
            try
            {
                Domeniu domeniu = db.Domenii.Find(id);
                if (TryUpdateModel(domeniu))
                {
                    domeniu.NumeDomeniu = requestDomeniu.NumeDomeniu;
                    db.SaveChanges();
                    TempData["message"] = "Domeniul " + requestDomeniu.NumeDomeniu + " a fost modificat!";
                    return RedirectToAction("Index");
                }

                return View(requestDomeniu);
            }
            catch (Exception e)
            {
                return View(requestDomeniu);
            }
        }


        //Stergere domeniu cu id-ul dat
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Domeniu domeniu = db.Domenii.Find(id);
            db.Domenii.Remove(domeniu);
            TempData["message"] = "Domeniul " + domeniu.NumeDomeniu + " a fost sters!";
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
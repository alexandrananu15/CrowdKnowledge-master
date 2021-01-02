using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CrowdKnowledge2.Models;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;
using Microsoft.Security.Application;

namespace CrowdKnowledge2.Controllers
{
    public class CapitoleController : Controller
    {
        private CrowdKnowledge2.Models.ApplicationDbContext db = new CrowdKnowledge2.Models.ApplicationDbContext();

        // GET: Afisare capitole corespunzatoare articolului cu id-ul parametru dat
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            ViewBag.afisareButoan = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoan = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(db.Articole.Find(id));
        }



        //Stergere capitol cu id-ul dat ca parametru 
        [HttpDelete]
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Capitol capt = db.Capitole.Find(id);
            if (capt.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                db.Capitole.Remove(capt);
                db.SaveChanges();
                TempData["message"] = "Capitolul a fost sters!";
                return Redirect("/Capitole/Index/" + capt.IDArticol);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest capitol!";
                return Redirect("/Capitole/Index/" + capt.IDArticol);            }
        }


        //Adaugare capitol
        //[HttpPost]
        //public ActionResult New(Capitol capt)
        //{
        //    capt.Data = DateTime.Now;
        //    try
        //    {
        //        db.Capitole.Add(capt);
        //        db.SaveChanges();
        //        TempData["message"] = "Capitolul " + capt.IDCapitol + " a fost adaugat!";
        //        return Redirect("/Articole/Show/" + capt.IDArticol);
        //    }

        //    catch (Exception e)
        //    {
        //        return Redirect("/Articole/Show/" + capt.IDArticol);
        //    }

        //}

        //Adaugare capitol nou pentru articolul cu id-ul dat ca parametru 
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New(int id)
        {
            ViewBag.IdCap = id;
            Capitol capitol = new Capitol();
            capitol.UserId = User.Identity.GetUserId();
            return View();
        }


        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        [ValidateInput(false)]
        public ActionResult New(Capitol capt)
        {
            capt.Data = DateTime.Now;
            capt.UserId = User.Identity.GetUserId();
            try
            {
                if (ModelState.IsValid)
                {
                    capt.Content = Sanitizer.GetSafeHtmlFragment(capt.Content);
                    db.Capitole.Add(capt);
                    db.SaveChanges();
                    TempData["message"] = "Capitolul a fost adaugat!";
                    //return Redirect("/Articole/Show/" + capt.IDArticol);
                    return Redirect("/Capitole/Index/" + capt.IDArticol);
                }
                else
                {
                    ViewBag.IdCap = capt.IDArticol;
                    return View(capt);
                    //return RedirectToAction("Show/" + capt.IDArticol, "Articole", capt);
                    //return Redirect("/Articole/Show/" + capt.IDArticol);
                }
            }

            catch (Exception e)
            {
                ViewBag.IdCap = capt.IDArticol;
                return View(capt);
                //return View("/Articole/Show/" + capt.IDArticol, capt);
                //return Redirect("/Articole/Show/" + capt.IDArticol);
            }

        }


        //Editare capitol
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Capitol capt = db.Capitole.Find(id);
            //ViewBag.Capitol = capt;
            if (capt.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
            {
                return View(capt);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui capitol!";
                return Redirect("/Capitole/Index/" + capt.IDArticol);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Capitol requestCapitol)
        {
            try
            {
                Capitol capt = db.Capitole.Find(id);
                if (capt.UserId == User.Identity.GetUserId() || User.IsInRole("Admin"))
                {
                    if (TryUpdateModel(capt))
                    {
                        capt.Content = Sanitizer.GetSafeHtmlFragment(requestCapitol.Content);
                        //capt.Content = requestCapitol.Content;
                        db.SaveChanges();
                        TempData["message"] = "Capitolul a fost modificat!";
                    }
                    return Redirect("/Capitole/Index/" + capt.IDArticol);
                }
                else
                {
                    TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui capitol deoarece nu va apartine!";
                    return Redirect("/Capitole/Index/" + capt.IDArticol);

                }
            }
            catch (Exception e)
            {
                return View();
            }

        }

        //Afisare capitol
        [Authorize(Roles = "User, Editor,Admin")]
        public ActionResult Show(int id)
        {
            
            Capitol capitol = db.Capitole.Find(id);
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(capitol);

        }

    }
}
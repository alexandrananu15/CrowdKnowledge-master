using CrowdKnowledge2.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Security.Application;
using AppContext = CrowdKnowledge2.Models.ApplicationDbContext;

namespace CrowdKnowledge2.Controllers
{
    public class ArticoleController : Controller
    {
        private ApplicationDbContext db = new CrowdKnowledge2.Models.ApplicationDbContext();


        // Afisare lista articole
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Index(string sort)
        {
            var articole = db.Articole.Include("Domeniu").Include("User");
            ViewBag.sortMe = sort;
           
            if (sort == null)
               articole = ViewBag.Articole = articole.OrderByDescending(a => a.Data);
            else
                articole = ViewBag.Articole = articole.OrderBy(a => a.TitluArticol);
            var search = "";
            if (Request.Params.Get("search") != null)
            {
                ViewBag.search = search = Request.Params.Get("search").Trim();
                //trim whitespace
                //from search string
                //Seacrh in articles (title and content)
                ViewBag.Articole = articole.Where(
                    at => at.TitluArticol.Contains(search)
                    || at.ContinutArticol.Contains(search)
                    );
            }

            //List of articles that contains
            //the search string either in the article title or content
            //List<int> mergeIds = articleIds.Union

            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }

            //Butoanul de adaugare articol este vizibil doar pentru Admin si Editor
            //Admin-ul are butoanul vizibil pentru orice articol,
            //iar Editorul il are vizibil doar pentru articolele care ii apartin
            //Utilizatorul cu rolul User nu o sa vada acest buton
            ViewBag.afisareButoan = false;
            //BUTOAN?
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoan = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View();
        }


        //Afisare articol cu id-ul dat
        [Authorize(Roles = "User,Editor,Admin")]
        public ActionResult Show(int id)
        {
            if (TempData.ContainsKey("message"))
            {
                ViewBag.Message = TempData["message"];
            }
            Articol articol = db.Articole.Find(id);

            //Butoanele de Edit si Delete sunt vizibile doar pentru Admin si Editor
            //Admin-ul are butoanele vizibile pentru orice articol,
            //iar Editorul le are vizibile doar pentru articolele care ii apartin
            //Utilizatorul cu rolul User nu o sa vada aceste butoane
            ViewBag.afisareButoane = false;
            if (User.IsInRole("Editor") || User.IsInRole("Admin"))
            {
                ViewBag.afisareButoane = true;
            }
            ViewBag.esteAdmin = User.IsInRole("Admin");
            ViewBag.utilizatorCurent = User.Identity.GetUserId();
            return View(articol);
        }


        //Adaugare articol nou
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult New()
        {
            Articol articol = new Articol();
            articol.Dom = GetAllCategories();

            // Preluam ID-ul utilizatorului curent
            articol.UserId = User.Identity.GetUserId();
            return View(articol);
        }


        [HttpPost]
        [Authorize(Roles = "Editor,Admin")]
        [ValidateInput(false)]
        public ActionResult New(Articol articol)
        {
            articol.Restrict = false;
            articol.Dom = GetAllCategories();
            articol.Data = DateTime.Now;
            articol.UserId = User.Identity.GetUserId();
            try
           {
                if (ModelState.IsValid)
                {
                    // Protect content from XSS
                    articol.ContinutArticol = Sanitizer.GetSafeHtmlFragment(articol.ContinutArticol);
                    db.Articole.Add(articol);
                    db.SaveChanges();
                    TempData["message"] = "Articolul a fost adaugat!";
                    return RedirectToAction("Index");
                }
                else
                {
                    return View(articol);
                }
            }
            catch (Exception e)
            {
                return View(articol);
            }
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public ActionResult Restrict(int id)
        {
            var art = db.Articole.Find(id);
            art.Restrict = !art.Restrict;
            
            if (TryUpdateModel(art))
            {
                db.SaveChanges();
            }

            return Redirect("/Articole/Show/" + id);
        }

        //Editare articol
        //Verificare ID user care doreste sa editeze un articol = ID user care a creat articolul
        //Sau daca utilizatorul este Admin
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Edit(int id)
        {
            Articol articol = db.Articole.Find(id);
            articol.Dom = GetAllCategories();

            if (articol.UserId == User.Identity.GetUserId() && articol.Restrict == false || User.IsInRole("Admin"))
            {
                return View(articol);
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui articol deoarece nu va apartine!";
                return RedirectToAction("Index");
            }
        }

        [HttpPut]
        [Authorize(Roles = "Editor,Admin")]
        [ValidateInput(false)]
        public ActionResult Edit(int id, Articol requestArticol)
        {
            requestArticol.Dom = GetAllCategories();

            try
            {
                if (ModelState.IsValid)
                {
                    Articol articol = db.Articole.Find(id);
                    if (articol.UserId == User.Identity.GetUserId() && articol.Restrict == false || User.IsInRole("Admin"))
                    {
                        if (TryUpdateModel(articol))
                        {
                            articol.TitluArticol = requestArticol.TitluArticol;
                            // Protect content from XSS
                            requestArticol.ContinutArticol = Sanitizer.GetSafeHtmlFragment(requestArticol.ContinutArticol);
                            articol.ContinutArticol = requestArticol.ContinutArticol;
                            articol.IDDomeniu = requestArticol.IDDomeniu;
                            db.SaveChanges();
                            TempData["message"] = "Articolul a fost modificat!";
                        }
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["message"] = "Nu aveti dreptul sa faceti modificari asupra acestui articol deoarece nu va apartine!";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    return View(requestArticol);
                }

            }
            catch (Exception e)
            {
                return View(requestArticol);
            }
        }


        //Stergere articol
        //Verificare ID user care doreste sa stearga un articol = ID user care a creat articolul
        //Sau daca utilizatorul este Admin
        [HttpDelete]
        [Authorize(Roles = "Editor,Admin")]
        public ActionResult Delete(int id)
        {
            Articol articol = db.Articole.Find(id);
            if (articol.UserId == User.Identity.GetUserId() && articol.Restrict == false || User.IsInRole("Admin"))
            {
                db.Articole.Remove(articol);
                db.SaveChanges();
                TempData["message"] = "Articolul a fost sters!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["message"] = "Nu aveti dreptul sa stergeti acest articol!";
                return RedirectToAction("Index");            }
        }
        


        [NonAction]
        public IEnumerable<SelectListItem> GetAllCategories()
        {
            var lista = new List<SelectListItem>();
            var domenii = from dom in db.Domenii
                             select dom;

            foreach (var domeniu in domenii)
            {
                lista.Add(new SelectListItem
                {
                    Value = domeniu.IDDomeniu.ToString(),
                    Text = domeniu.NumeDomeniu.ToString()
                });
            }
            return lista;
        }
    }
}
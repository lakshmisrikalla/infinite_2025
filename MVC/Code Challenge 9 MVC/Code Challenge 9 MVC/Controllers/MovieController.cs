using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Code_Challenge_9_MVC.Repository;
using Code_Challenge_9_MVC.Models;

namespace Code_Challenge_9_MVC.Controllers
{
        public class MovieController : Controller
        {
            private MovieRepository repo = new MovieRepository();

            public ActionResult MainPage() => View();

            public ActionResult Index() => View(repo.GetAll());

            public ActionResult Create() => View();

            [HttpPost]
            public ActionResult Create(Movie m)
            {
                repo.Add(m);
                return RedirectToAction("Index");
            }

        public ActionResult Edit()
        {
            return View(); // Initial view with ID input
        }

        [HttpPost]
        public ActionResult Edit(int id)
        {
            var movie = repo.GetById(id);
            if (movie == null)
            {
                ViewBag.Error = $"Movie with ID {id} not found.";
                return View();
            }
            return View(movie); // Pass movie to view for editing
        }

        [HttpPost]
        public ActionResult EditForm(Movie m)
        {
            if (ModelState.IsValid)
            {
                repo.Update(m);
                ViewBag.Message = $"Movie with ID {m.Mid} updated.";
                return View("Edit", m); // Return to same view with success message
            }

            ViewBag.Error = "Invalid data. Please check your inputs.";
            return View("Edit", m);
        }


        public ActionResult Delete() => View();

            [HttpPost]
            public ActionResult Delete(int id)
            {
                var movie = repo.GetById(id);
                if (movie == null)
                {
                    ViewBag.Error = $"Movie with ID {id} not found.";
                    return View();
                }

                repo.Delete(id);
                ViewBag.Message = $"Movie with ID {id} deleted.";
                return View();
            }

            public ActionResult ByYear(int year) =>
                View(repo.GetByYear(year));

            public ActionResult ByDirector(string name) =>
                View(repo.GetByDirector(name));

        public ActionResult Main()
        {
            return View();
        }
    }
  

}

using System;
using System.Web.Mvc;
using Hands_On_MVC.Models;

namespace Hands_On_MVC.Controllers
{
    public class JobApplicationController : Controller
    {
        // GET: JobApplication/Application
        public ActionResult Application()
        {
            return View();
        }

        // POST: JobApplication/Application
        [HttpPost]
        public ActionResult Application(JobApplication model)
        {
            if (ModelState.IsValid)
            {
                // You can add save logic here if needed
                return RedirectToAction("Success");
            }

            return View(model);
        }

        public ActionResult Success()
        {
            return View();
        }
    }
}

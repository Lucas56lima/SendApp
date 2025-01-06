using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace SendApp.Controllers
{
    public class SchedulingController : Controller
    {
        // GET: SchedulingController
        public ActionResult Index()
        {
            return View();
        }

        // GET: SchedulingController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: SchedulingController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SchedulingController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SchedulingController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: SchedulingController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: SchedulingController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: SchedulingController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

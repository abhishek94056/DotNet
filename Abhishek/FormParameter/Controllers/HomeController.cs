using FormParameter.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FormParameter.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ShowData(String nm, String gn, String ag)
        {
            ViewBag.Name = nm;
            ViewBag.Gender = gn;
            ViewBag.age = ag;
            return View();
        }

        public IActionResult MyForm()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

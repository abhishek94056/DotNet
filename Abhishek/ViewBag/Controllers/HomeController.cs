using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ViewBag.Models;

namespace ViewBag.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Id = 1;
            ViewBag.Name = "Abhishek";
            ViewBag.Email = "abhishek94056@gmail.com";
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

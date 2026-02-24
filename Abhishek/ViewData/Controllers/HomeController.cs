using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using ViewData.Models;

namespace ViewData.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["pId"] = 1;
            ViewData["pName"] = "Laptop";
            ViewData["pPrice"] = 599;
            ViewData["pQty"] = 7;
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

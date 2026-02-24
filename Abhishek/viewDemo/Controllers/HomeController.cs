using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;
using viewDemo.Models;

namespace viewDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Student s = new Student()
            {
                r_no = 1,
                name = "Krishna",
                gender = "Male",
                course = "Dharma",
                fees = 00000
            };
            return View(s);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
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

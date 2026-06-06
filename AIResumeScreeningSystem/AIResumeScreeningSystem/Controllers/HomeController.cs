using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AIResumeScreeningSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return Redirect("/Admin/Dashboard");
                if (User.IsInRole("Recruiter"))
                    return Redirect("/Recruiter/Dashboard");
                if (User.IsInRole("Candidate"))
                    return Redirect("/Candidate/Dashboard");
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
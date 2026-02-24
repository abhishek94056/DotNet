using Microsoft.AspNetCore.Mvc;
using StronglyTypeView.Models;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Xml.Linq;

namespace StronglyTypeView.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            Product p = new Product()
            {
                pId = 1,
                pName="Book",
                pPrice=299,
                pQty=2
            };
            return View(p);
        }

        public IActionResult Customer()
        {
            List<Customers> customers = new List<Customers>()
            {
                new Customers(){c_Id=1, Name="Abhishek", Email="abhishek94056@gmail.com" },
                new Customers(){c_Id=1, Name="Abhishek", Email="abhishek94056@gmail.com" },
                new Customers(){c_Id=1, Name="Abhishek", Email="abhishek94056@gmail.com" },
                new Customers(){c_Id=1, Name="Abhishek", Email="abhishek94056@gmail.com" }
            };
            return View(customers);
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

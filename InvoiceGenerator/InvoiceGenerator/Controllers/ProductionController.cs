using InvoiceGenerator.Filters;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    public class ProductionController : Controller
    {
        [RequireLogin]
        public IActionResult ProductionView()
        {
            return View();
        }
    }
}

using InvoiceGenerator.Models;
using InvoiceGenerator.Interfaces;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc;   //Controller functionality

namespace InvoiceGenerator.Controllers
{          
    public class CompanyController : Controller  //(C) Controller -> Base class for MVC controllers
    {
        //Dependency Injection (Service)
        private readonly ICompanyService _svc;
        public CompanyController(ICompanyService svc) => _svc = svc;

        // GET: company records
        public IActionResult CompanyView()    //(I) IActionResult -> Represents action result
        {
            var list = _svc.GetAll();
            return View(list);       //(M) View()	-> Returns UI view
        }

        // POST: Save (Insert or Update)
        [HttpPost]  //form submit
        [ValidateAntiForgeryToken]  //security (CSRF protection)
        public IActionResult Save(CompanyModel model)
        {
            if (!ModelState.IsValid)   //if ModelState is fail return error
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });

            if (model.CompanyId == 0)     //New record (Insert)
                _svc.Insert(model);
            else                          //Existing record (Update)
                _svc.Update(model);

            return Json(new { success = true });  //Response to AJAX in JSON format
        }

        // GET:
        // one record for Edit modal
        [HttpGet]
        public IActionResult GetById(int Id)
        {
            var c = _svc.GetById(Id);
            if (c == null) return NotFound();
            return Json(c);
        }

        // POST: Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new { success = true });
        }
    }
}
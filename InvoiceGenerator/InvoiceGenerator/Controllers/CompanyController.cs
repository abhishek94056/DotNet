using InvoiceGenerator.Filters;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using iText.Commons.Utils;
using Microsoft.AspNetCore.Mvc;   

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class CompanyController : Controller  
    {
        
        private readonly ICompanyService _svc;
        public CompanyController(ICompanyService svc) => _svc = svc;

        // GET: company records
        public IActionResult CompanyView()    
        {
            var list = _svc.GetAll();
            return View(list);       
        }

        // POST: Save (Insert or Update)
        [HttpPost] 
        [ValidateAntiForgeryToken] 
        public IActionResult Save(CompanyModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });
            if (model.CompanyId == 0)
            {
                int result = _svc.Insert(model); 

                if (result == -1)
                {
                    return Json(new
                    {
                        success = false,
                        message = "GSTIN already exists." 
                    });
                }
            }
            else
            {
                _svc.Update(model);
            }

            return Json(new
            {
                success = true,
                message = "Company saved successfully."
            });
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
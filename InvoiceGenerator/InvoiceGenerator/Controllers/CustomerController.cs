// Controllers/CustomerController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class CustomerController : Controller
    {
        private readonly CustomerService _svc;

        public CustomerController(CustomerService svc) => _svc = svc;

        // GET: /Customer
        public IActionResult CustomerView()
        {
            var list = _svc.GetAll();
            return View(list);
        }

        // GET: /Customer/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var c = _svc.GetById(id);
            if (c == null) return NotFound();
            return Json(new
            {
                customerId = c.CustomerId,
                customer = c.Customer,
                address = c.Address,
                stateCode = c.StateCode,
                state = c.State,    
                gstin = c.GSTIN,
                //date = c.Date?.ToString("yyyy-MM-dd"),
                createdBy = c.CreatedBy
            }); 
        }

        // POST: /Customer/Save
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(CustomerModel model)
        //{

        //    //model.Date = DateTime.Now;
        //    model.CreatedBy = SessionHelper.GetUserName(HttpContext.Session);
        //    if (!ModelState.IsValid)
        //        return Json(new
        //        {
        //            success = false,
        //            message = string.Join(" | ", ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage))
        //        });

        //    // Get logged-in user name from session
        //    string createdBy = SessionHelper.GetUserName(HttpContext.Session);

        //    if (model.CustomerId == 0)
        //    {
        //        var (success, message, _) = _svc.Insert(model, createdBy);
        //        return Json(new { success, message });
        //    }
        //    else
        //    {
        //        _svc.Update(model, createdBy);
        //        return Json(new
        //        {
        //            success = true,
        //            message = "Customer updated successfully."
        //        });
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(CustomerModel model)
        {
            model.CreatedBy = SessionHelper.GetUserName(HttpContext.Session);

            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });

            string createdBy = SessionHelper.GetUserName(HttpContext.Session);

            if (model.CustomerId == 0)
            {
                var (success, message, _) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }
            else
            {
                var (success, message) = _svc.Update(model, createdBy); // ✅ FIX

                return Json(new
                {
                    success,
                    message
                });
            }
        }
        // POST: /Customer/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Customer deleted successfully."
            });
        }
    }
}
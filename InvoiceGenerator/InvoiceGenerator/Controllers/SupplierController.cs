// Controllers/SupplierController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class SupplierController : Controller
    {
        private readonly SupplierService _svc;

        public SupplierController(SupplierService svc) => _svc = svc;

        // GET: /Supplier
        public IActionResult SupplierView()
        {
            var list = _svc.GetAll();
            return View(list);
        }

        // GET: /Supplier/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var s = _svc.GetById(id);
            if (s == null) return NotFound();
            return Json(new
            {
                supplierId = s.SupplierId,
                supplier = s.Supplier,
                address = s.Address,
                stateCode = s.StateCode,
                state = s.State,
                gstin = s.GSTIN,
                //date = s.Date.ToString("yyyy-MM-dd"),
                createdBy = s.CreatedBy
            });
        }

        // POST: /Supplier/Save
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(SupplierModel model)
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

        //    string createdBy = SessionHelper.GetUserName(HttpContext.Session);

        //    if (model.SupplierId == 0)
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
        //            message = "Supplier updated successfully."
        //        });
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(SupplierModel model)
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

            if (model.SupplierId == 0)
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
        // POST: /Supplier/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Supplier deleted successfully."
            });
        }
    }
}
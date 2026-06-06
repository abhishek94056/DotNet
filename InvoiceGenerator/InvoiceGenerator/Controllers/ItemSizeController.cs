// Controllers/ItemSizeController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    //[RequireAdmin]
    public class ItemSizeController : Controller
    {
        private readonly ItemSizeService _svc;

        public ItemSizeController(ItemSizeService svc) => _svc = svc;

        // GET: /ItemSize
        public IActionResult ItemSizeView()
        {
            var list = _svc.GetAll();
            var depts = _svc.GetDepartments();
            ViewBag.Departments = depts;
            return View(list);
        }
        //[HttpGet]
        //public IActionResult GetAll()
        //{
        //    var list = _svc.GetAll();

        //    return Json(new
        //    {
        //        success = true,
        //        data = list
        //    });
        //}
        // GET: /ItemSize/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var m = _svc.GetById(id);
            if (m == null) return NotFound();
            return Json(new
            {
                sizeId = m.SizeId,
                departmentId = m.DepartmentId,
                itemSize_Code = m.ItemSize_Code,
                item_Size = m.Item_Size,
                rate = m.Rate,
                //date = m.Date.ToString("yyyy-MM-dd"),
                createdBy = m.CreatedBy,
                mIN_Stock = m.MIN_Stock,
                mAX_Stock = m.MAX_Stock
            });
        }

        // POST: /ItemSize/Save
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(ItemSizeModel model)
        //{
        //    //model.Date = DateTime.Now.Date;    
        //    model.CreatedBy = SessionHelper.GetUserName(HttpContext.Session);
        //    if (!ModelState.IsValid)
        //        return Json(new
        //        {
        //            success = false,
        //            message = string.Join(" | ", ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage))
        //        });

        //    // Validate MIN < MAX
        //    if (model.MIN_Stock >= model.MAX_Stock)
        //        return Json(new
        //        {
        //            success = false,
        //            message = "MIN Stock must be less than MAX Stock."
        //        });

        //    string createdBy =
        //        SessionHelper.GetUserName(HttpContext.Session);

        //    if (model.SizeId == 0)
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
        //            message = "Item Size updated successfully."
        //        });
        //    }
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ItemSizeModel model)
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

            // Validate MIN < MAX
            if (model.MIN_Stock >= model.MAX_Stock)
                return Json(new
                {
                    success = false,
                    message = "MIN Stock must be less than MAX Stock."
                });

            string createdBy = SessionHelper.GetUserName(HttpContext.Session);

            if (model.SizeId == 0)
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
        // POST: /ItemSize/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Item Size deleted successfully."
            });
        }
    }
}
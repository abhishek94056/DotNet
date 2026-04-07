// Controllers/ItemController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class ItemController : Controller
    {
        private readonly IItemService _svc;

        public ItemController(IItemService svc) => _svc = svc;

        // GET: /Item
        public IActionResult ItemView()
        {
            var list = _svc.GetAll();
            return View(list);
        }

        // GET: fetch one record for Edit modal
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var item = _svc.GetById(id);
            if (item == null) return NotFound();
            return Json(item);
        }

        // POST: Save (Insert or Update)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Save(ItemModel model)
        //{
        //    if (!ModelState.IsValid)
        //        return Json(new
        //        {
        //            success = false,
        //            errors = ModelState.Values
        //                .SelectMany(v => v.Errors)
        //                .Select(e => e.ErrorMessage)
        //        });

        //    if (model.ItemId == 0)
        //        _svc.Insert(model);
        //    else
        //        _svc.Update(model);

        //    return Json(new { success = true });
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ItemModel model)
        {
            // ❌ validation fail
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });

            // 🔹 Insert
            if (model.ItemId == 0)
            {
                int result = _svc.Insert(model);

                // ❌ Duplicate ItemCode
                if (result == -1)
                {
                    return Json(new
                    {
                        success = false,
                        message = "Item Code already exists."
                    });
                }
            }
            else
            {
                // 🔹 Update
                _svc.Update(model);
            }

            // ✅ success
            return Json(new
            {
                success = true,
                message = "Item saved successfully."
            });
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
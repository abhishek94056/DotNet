// Controllers/ItemController.cs
using Microsoft.AspNetCore.Mvc;
using InvoiceGenerator.Models;
using InvoiceGenerator.Interfaces;

namespace InvoiceGenerator.Controllers
{
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(ItemModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                });

            if (model.ItemId == 0)
                _svc.Insert(model);
            else
                _svc.Update(model);

            return Json(new { success = true });
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
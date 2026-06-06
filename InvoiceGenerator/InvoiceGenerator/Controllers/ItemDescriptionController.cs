// Controllers/ItemDescriptionController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    //[RequireAdmin]
    public class ItemDescriptionController : Controller
    {

        private readonly ItemDescriptionService _svc;
        private readonly ItemSizeService _sizeSvc;

        public ItemDescriptionController(
            ItemDescriptionService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /ItemDescription
        //public IActionResult ItemDescriptionView() => View();

        // 🔹 Blister View
        public IActionResult Blister()
        {
            ViewBag.Department = "Blister";
            return View();
        }

        // 🔹 Extrusion View
        public IActionResult Extrusion()
        {
            ViewBag.Department = "Extrusion";
            return View();
        }

        public IActionResult Moulding()
        {
            ViewBag.Department = "Moulding";
            return View();
        }

        public IActionResult PPBox()
        {
            ViewBag.Department = "PPBox";
            return View();
        }

        public IActionResult PVC()
        {
            ViewBag.Department = "PVC";
            return View();
        }

        // GET: /ItemDescription/GetAll  — called by jQuery AJAX
        [HttpGet]
        public IActionResult GetAll(int departmentId)
            => Json(_svc.GetAll(departmentId));

        // GET: /ItemDescription/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var m = _svc.GetById(id);
            if (m == null) return NotFound();
            return Json(m);
        }

        //GET: /ItemDescription/GetDropdowns
       [HttpGet]
        public IActionResult GetDropdowns()
        {
            return Json(new
            {
                departments = _sizeSvc.GetDepartments(),
                machines = _svc.GetMachines(),
                packing = _svc.GetPackingTypes(),
                innerPacking = _svc.GetInnerPackingTypes(),
                sizes = _sizeSvc.GetAll()
                    .Select(s => new { s.SizeId, s.ItemSize_Code, s.Item_Size })
            });
        }


        [HttpGet]
        public IActionResult GetItemSizes()
        {
            var data = _sizeSvc.GetAll()
                .Select(s => new { s.SizeId, s.DepartmentId, s.ItemSize_Code });

            return Json(data);
        }

        // POST: /ItemDescription/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] ItemDescriptionModel model)
        {
            if (!ModelState.IsValid)
                return Json(new
                {
                    success = false,
                    message = string.Join(" | ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage))
                });

            if (model.MIN_Stock >= model.MAX_Stock)
                return Json(new
                {
                    success = false,
                    message = "MIN Stock must be less than MAX Stock."
                });

            string createdBy = SessionHelper.GetUserName(HttpContext.Session);

            if (model.ItemId == 0)
            {
                var (success, message, _) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }

            _svc.Update(model, createdBy);
            return Json(new
            {
                success = true,
                message = "Item updated successfully."
            });
        }

        // POST: /ItemDescription/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Item deleted successfully."
            });
        }
    }
}
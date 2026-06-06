// Controllers/RawMaterialController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class RawMaterialController : Controller
    {
        private readonly RawMaterialService _svc;
        private readonly ItemSizeService _sizeSvc;

        public RawMaterialController(
            RawMaterialService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /RawMaterial
        public IActionResult RawMaterialView() => View();

        // GET: /RawMaterial/GetAll?deptId=1&monthId=5
        [HttpGet]
        public IActionResult GetAll(int deptId, int monthId)
            => Json(_svc.GetAll(deptId, monthId));

        // GET: /RawMaterial/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /RawMaterial/GetSizesByDept?deptId=1
        [HttpGet]
        public IActionResult GetSizesByDept(int deptId)
            => Json(_svc.GetSizesByDept(deptId));

        // POST: /RawMaterial/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] RawMaterialModel model)
        {
            if (model.SizeId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select an Item Size."
                });

            if (model.Quantity <= 0)
                return Json(new
                {
                    success = false,
                    message = "Quantity must be greater than 0."
                });

            if (model.RM_Rate < 0)
                return Json(new
                {
                    success = false,
                    message = "RM Rate cannot be negative."
                });

            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            if (model.SrNo == 0)
            {
                var (success, message, _) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }

            _svc.Update(model, createdBy);
            return Json(new
            {
                success = true,
                message = "Raw Material updated successfully."
            });
        }

        // POST: /RawMaterial/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Raw Material deleted successfully."
            });
        }
    }
}
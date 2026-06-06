// Controllers/SupplierPOStatusController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class SupplierPOStatusController : Controller
    {
        private readonly SupplierPOStatusService _svc;
        private readonly ItemSizeService _sizeSvc;

        public SupplierPOStatusController(
            SupplierPOStatusService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult SupplierPOStatusView() => View();

        [HttpGet]
        public IActionResult GetAll(
            int deptId, int sizeId, int monthId, int yearId)
            => Json(_svc.GetAll(deptId, sizeId, monthId, yearId));

        [HttpGet]
        public IActionResult GetAllMonth(
            int deptId, int monthId, int yearId)
            => Json(_svc.GetAllMonth(deptId, monthId, yearId));

        [HttpGet]
        public IActionResult GetReporting(
            int deptId, int monthId, int yearId)
            => Json(_svc.GetReporting(deptId, monthId, yearId));

        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        [HttpGet]
        public IActionResult GetSuppliers()
            => Json(_svc.GetSuppliers());

        [HttpGet]
        public IActionResult GetSizesByDept(int deptId)
            => Json(_svc.GetSizesByDept(deptId));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] SupplierPOStatusModel model)
        {
            if (model.SupplierId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Supplier."
                });
            if (model.SizeId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Size."
                });
            if (model.RM_Flag == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Transaction Type."
                });
            if (model.RM_Qty <= 0)
                return Json(new
                {
                    success = false,
                    message = "Quantity must be greater than 0."
                });

            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            var (success, message) = _svc.Save(model, createdBy);
            return Json(new { success, message });
        }
    }
}
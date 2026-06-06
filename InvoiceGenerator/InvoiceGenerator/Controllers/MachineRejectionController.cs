// Controllers/MachineRejectionController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class MachineRejectionController : Controller
    {
        private readonly MachineRejectionService _svc;
        private readonly ItemSizeService _sizeSvc;

        public MachineRejectionController(
            MachineRejectionService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult MachineRejectionView() => View();

        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        [HttpGet]
        public IActionResult GetShifts()
            => Json(_svc.GetShifts());

        [HttpGet]
        public IActionResult GetMachinesByDept(int deptId)
            => Json(_svc.GetMachinesByDept(deptId));

        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDept(deptId));

        [HttpGet]
        public IActionResult GetRejectionTypes(int deptId)
        => Json(_svc.GetRejectionTypes(deptId));

        [HttpGet]
        public IActionResult GetOperatorsByDept(int deptId)
            => Json(_svc.GetOperatorsByDept(deptId));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] MachineRejectionModel model)
        {
            if (model.ShiftId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Shift."
                });
            if (model.MachineId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Machine."
                });
            if (model.ItemId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select an Item."
                });
            if (model.RejectionId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Rejection Reason."
                });
            if (model.Rejection_Qty <= 0)
                return Json(new
                {
                    success = false,
                    message = "Rejection Qty must be greater than 0."
                });
            if (string.IsNullOrEmpty(model.Date))
                return Json(new
                {
                    success = false,
                    message = "Date is required."
                });

            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            if (model.SrNo == 0)
            {
                var (success, message) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }

            _svc.Update(model, createdBy);
            return Json(new
            {
                success = true,
                message = "Rejection data updated successfully."
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Rejection record deleted."
            });
        }
    }
}
// Controllers/MachineDowntimeController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class MachineDowntimeController : Controller
    {
        private readonly MachineDowntimeService _svc;
        private readonly ItemSizeService _sizeSvc;

        public MachineDowntimeController(
            MachineDowntimeService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult MachineDowntimeView() => View();

        [HttpGet]
        public IActionResult GetAll(int deptId, int monthId)
            => Json(_svc.GetAll(deptId, monthId));

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
        public IActionResult GetDowntimeReasons()
            => Json(_svc.GetDowntimeReasons());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] MachineDowntimeModel model)
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
            if (model.MC_StatusId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select Machine Status."
                });
            if (model.ItemId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select an Item."
                });
            if (model.DownTime_ReasonId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Downtime Reason."
                });
            if (model.DownTime <= 0)
                return Json(new
                {
                    success = false,
                    message = "Downtime must be greater than 0."
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
                message = "Downtime record updated successfully."
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
                message = "Downtime record deleted."
            });
        }
    }
}
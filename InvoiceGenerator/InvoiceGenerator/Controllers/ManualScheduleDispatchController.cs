// Controllers/ManualScheduleDispatchController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class ManualScheduleDispatchController : Controller
    {
        private readonly ManualScheduleDispatchService _svc;
        private readonly ItemSizeService _sizeSvc;

        public ManualScheduleDispatchController(
            ManualScheduleDispatchService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult ManualScheduleDispatchView() => View();

        // GET: /ManualScheduleDispatch/GetAll?deptId=1&monthId=6
        [HttpGet]
        public IActionResult GetAll(int deptId, int monthId)
            => Json(_svc.GetAll(deptId, monthId));

        // GET: /ManualScheduleDispatch/GetReport?deptId=1&customerId=2&monthId=6&yearId=2026
        [HttpGet]
        public IActionResult GetReport(
            int deptId, int customerId, int monthId, int yearId)
            => Json(_svc.GetReport(deptId, customerId, monthId, yearId));

        // GET: /ManualScheduleDispatch/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /ManualScheduleDispatch/GetCustomers
        [HttpGet]
        public IActionResult GetCustomers()
            => Json(_svc.GetCustomers());

        // GET: /ManualScheduleDispatch/GetItemsByDeptCustomer?deptId=1&customerId=2
        [HttpGet]
        public IActionResult GetItemsByDeptCustomer(
            int deptId, int customerId)
            => Json(_svc.GetItemsByDeptCustomer(deptId, customerId));

        // POST: /ManualScheduleDispatch/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save(
            [FromForm] ManualScheduleDispatchModel model)
        {
            if (model.CustomerId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Customer."
                });
            if (model.ItemId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select an Item."
                });
            if (model.Req_Quantity < 0)
                return Json(new
                {
                    success = false,
                    message = "Req Quantity cannot be negative."
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
                message = "Record updated successfully."
            });
        }

        // POST: /ManualScheduleDispatch/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Record deleted."
            });
        }
    }
}
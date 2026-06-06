// Controllers/CustomerPOScheduleController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class CustomerPOScheduleController : Controller
    {
        private readonly CustomerPOScheduleService _svc;
        private readonly ItemSizeService _sizeSvc;

        public CustomerPOScheduleController(
            CustomerPOScheduleService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /CustomerPOSchedule
        public IActionResult CustomerPOScheduleView() => View();

        // GET: /CustomerPOSchedule/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /CustomerPOSchedule/GetPOStatus?deptId=1
        [HttpGet]
        //public IActionResult GetPOStatus(int deptId)
        //    => Json(_svc.GetPOStatus(deptId));

        // GET: /CustomerPOSchedule/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /CustomerPOSchedule/GetCustomers
        [HttpGet]
        public IActionResult GetCustomers()
            => Json(_svc.GetCustomers());

        // GET: /CustomerPOSchedule/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDept(deptId));

        // GET: /CustomerPOSchedule/GetItemRate?itemId=1
        [HttpGet]
        public IActionResult GetItemRate(int itemId)
            => Json(new { rate = _svc.GetItemRate(itemId) });

        // POST: /CustomerPOSchedule/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] CustomerPOScheduleModel model)
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

            if (model.Quantity <= 0)
                return Json(new
                {
                    success = false,
                    message = "Quantity must be greater than 0."
                });

            if (string.IsNullOrEmpty(model.PO_Number))
                return Json(new
                {
                    success = false,
                    message = "PO Number is required."
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
                message = "PO Schedule updated successfully."
            });
        }

        // POST: /CustomerPOSchedule/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "PO Schedule deleted successfully."
            });
        }
    }
}
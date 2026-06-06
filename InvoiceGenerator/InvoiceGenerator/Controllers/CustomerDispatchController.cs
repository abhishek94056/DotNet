// Controllers/CustomerDispatchController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class CustomerDispatchController : Controller
    {
        private readonly CustomerDispatchService _svc;
        private readonly ItemSizeService _sizeSvc;

        public CustomerDispatchController(
            CustomerDispatchService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult CustomerDispatchView() => View();

        // GET: /CustomerDispatch/GetAll?deptId=1&monthId=5
        [HttpGet]
        public IActionResult GetAll(int deptId, int monthId)
            => Json(_svc.GetAll(deptId, monthId));

        // GET: /CustomerDispatch/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /CustomerDispatch/GetCustomers
        [HttpGet]
        public IActionResult GetCustomers()
            => Json(_svc.GetCustomers());

        // GET: /CustomerDispatch/GetItemsByDeptCustomer?deptId=1&customerId=2
        [HttpGet]
        public IActionResult GetItemsByDeptCustomer(int deptId, int customerId)
            => Json(_svc.GetItemsByDeptCustomer(deptId, customerId));

        // GET: /CustomerDispatch/GetItemDescription?itemId=1
        [HttpGet]
        public IActionResult GetItemDescription(int itemId)
        {
            var result = _svc.GetItemDescription(itemId);
            return result == null ? NotFound() : Json(result);
        }

        // GET: /CustomerDispatch/GetProduceQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetProduceQty(int deptId, int itemId)
            => Json(new { produce_Qty = _svc.GetProduceQty(deptId, itemId) });

        // GET: /CustomerDispatch/GetDispatchQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetDispatchQty(int deptId, int itemId)
            => Json(new { dispatch_Qty = _svc.GetDispatchQty(deptId, itemId) });

        // POST: /CustomerDispatch/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] CustomerDispatchModel model)
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
            if (model.DepartmentId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Department."
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
                message = "Dispatch updated successfully."
            });
        }

        // POST: /CustomerDispatch/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Dispatch record deleted."
            });
        }
    }
}
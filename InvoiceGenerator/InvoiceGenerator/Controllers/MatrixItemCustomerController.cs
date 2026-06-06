using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class MatrixItemCustomerController : Controller
    {
        private readonly MatrixItemCustomerService _svc;
        private readonly ItemSizeService _sizeSvc;

        public MatrixItemCustomerController(
            MatrixItemCustomerService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /MatrixItemCustomer
        public IActionResult MatrixItemCustomerView() => View();

        // GET: /MatrixItemCustomer/GetAll
        [HttpGet]
        public IActionResult GetAll(int departmentId)
            => Json(_svc.GetAll(departmentId));

        // GET: /MatrixItemCustomer/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var m = _svc.GetById(id);
            if (m == null) return NotFound();
            return Json(m);
        }

        // GET: /MatrixItemCustomer/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /MatrixItemCustomer/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDepartment(deptId));

        // GET: /MatrixItemCustomer/GetCustomers
        [HttpGet]
        public IActionResult GetCustomers()
            => Json(_svc.GetAllCustomers());

        // POST: /MatrixItemCustomer/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] MatrixItemCustomerFormModel model)
        {

            //model.Date = DateTime.Now;
            model.CreatedBy = SessionHelper.GetUserName(HttpContext.Session);
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

            if (model.DepartmentId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Department."
                });

            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            if (model.MatrixId == 0)
            {
                var (success, message, _) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }

            _svc.Update(model, createdBy);
            return Json(new
            {
                success = true,
                message = "Matrix updated successfully."
            });
        }

        // POST: /MatrixItemCustomer/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Record deleted successfully."
            });
        }
    }
}
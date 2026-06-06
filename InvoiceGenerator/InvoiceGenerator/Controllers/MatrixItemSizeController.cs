// Controllers/MatrixItemSizeController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireAdmin]
    public class MatrixItemSizeController : Controller
    {
        private readonly MatrixItemSizeService _svc;
        private readonly ItemSizeService _sizeSvc;

        public MatrixItemSizeController(
            MatrixItemSizeService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /MatrixItemSize
        public IActionResult MatrixItemSizeView() => View();

        // GET: /MatrixItemSize/GetAll
        [HttpGet]
        public IActionResult GetAll(int departmentId)
            => Json(_svc.GetAll(departmentId));

        // GET: /MatrixItemSize/GetById?id=1
        [HttpGet]
        public IActionResult GetById(int id)
        {
            var m = _svc.GetById(id);
            if (m == null) return NotFound();
            return Json(m);
        }

        // GET: /MatrixItemSize/GetDepartments
        //[HttpGet]
        //public IActionResult GetDepartments()
        //    => Json(_sizeSvc.GetDepartments());

        [HttpGet]
        public IActionResult GetDepartments()
        {
            var data = _sizeSvc.GetDepartments();  
            return Json(data);
        }

        // GET: /MatrixItemSize/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDepartment(deptId));

        // GET: /MatrixItemSize/GetSizesByDept?deptId=1
        [HttpGet]
        public IActionResult GetSizesByDept(int deptId)
            => Json(_svc.GetSizesByDepartment(deptId));

        // POST: /MatrixItemSize/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] MatrixItemSizeFormModel model)
        {
            //model.Date = DateTime.Now;
            model.CreatedBy = SessionHelper.GetUserName(HttpContext.Session);
            if (model.SizeId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Size."
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

        // POST: /MatrixItemSize/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "Matrix record deleted."
            });
        }
    }
}
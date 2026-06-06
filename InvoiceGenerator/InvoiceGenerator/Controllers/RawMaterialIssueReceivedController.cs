//// Controllers/RawMaterialIssueReceivedController.cs

//using InvoiceGenerator.Filters;
//using InvoiceGenerator.Helper;
//using InvoiceGenerator.Models;
//using InvoiceGenerator.Services;
//using Microsoft.AspNetCore.Mvc;

//namespace InvoiceGenerator.Controllers
//{
//    [RequireLogin]
//    public class RawMaterialIssueReceivedController : Controller
//    {
//        private readonly RawMaterialIssueReceivedService _svc;
//        private readonly ItemSizeService _sizeSvc;

//        public RawMaterialIssueReceivedController(
//            RawMaterialIssueReceivedService svc,
//            ItemSizeService sizeSvc)
//        {
//            _svc = svc;
//            _sizeSvc = sizeSvc;
//        }

//        // GET: /RawMaterialIssueReceived
//        public IActionResult RawMaterialIssueReceivedView()
//        {
//            return View();
//        }

//        // GET: /RawMaterialIssueReceived/GetAll?deptId=1
//        [HttpGet]
//        public IActionResult GetAll(int deptId)
//        {
//            return Json(_svc.GetAll(deptId));
//        }

//        // GET: /RawMaterialIssueReceived/GetDepartments
//        [HttpGet]
//        public IActionResult GetDepartments()
//        {
//            return Json(_svc.GetDepartments());
//        }

//        //Item Size is department-wise

//        [HttpGet]
//        public IActionResult GetItemsByDept(int deptId)
//        {
//            return Json(_svc.GetItemsByDept(deptId));
//        }

//        // POST: /RawMaterialIssueReceived/Save
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Save(
//            [FromForm] RawMaterialIssueReceivedModel model)
//        {
//            if (model.DepartmentId == 0)
//            {
//                return Json(new
//                {
//                    success = false,
//                    message = "Please select Department."
//                });
//            }

//            if (model.SizeId == 0)
//            {
//                return Json(new
//                {
//                    success = false,
//                    message = "Please select Size."
//                });
//            }

//            if (model.Quantity <= 0)
//            {
//                return Json(new
//                {
//                    success = false,
//                    message = "Quantity must be greater than 0."
//                });
//            }

//            if (string.IsNullOrEmpty(model.Date))
//            {
//                return Json(new
//                {
//                    success = false,
//                    message = "Please select Date."
//                });
//            }

//            string createdBy =
//                SessionHelper.GetUserName(HttpContext.Session);

//            if (model.SrNo == 0)
//            {
//                _svc.Insert(model, createdBy);

//                return Json(new
//                {
//                    success = true,
//                    message = "Record saved successfully."
//                });
//            }

//            _svc.Update(model, createdBy);

//            return Json(new
//            {
//                success = true,
//                message = "Record updated successfully."
//            });
//        }

//        // POST: /RawMaterialIssueReceived/Delete
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public IActionResult Delete(int srNo)
//        {
//            _svc.Delete(srNo);

//            return Json(new
//            {
//                success = true,
//                message = "Record deleted successfully."
//            });
//        }
//    }
//}


// Controllers/RawMaterialIssueController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class RawMaterialIssueReceivedController : Controller
    {
        private readonly RawMaterialIssueReceivedService _svc;
        private readonly ItemSizeService _sizeSvc;

        public RawMaterialIssueReceivedController(
            RawMaterialIssueReceivedService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult RawMaterialIssueReceivedView() => View();

        // GET: /RawMaterialIssue/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /RawMaterialIssue/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /RawMaterialIssue/GetSizesByDept?deptId=1
        [HttpGet]
        public IActionResult GetSizesByDept(int deptId)
            => Json(_svc.GetSizesByDept(deptId));

        // POST: /RawMaterialIssue/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] RawMaterialIssueReceivedModel model)
        {
            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            if (model.SrNo == 0)
            {
                var (success, message) = _svc.Insert(model, createdBy);
                return Json(new { success, message });
            }

            var (ok, msg) = _svc.Update(model, createdBy);
            return Json(new { success = ok, message = msg });
        }

        // POST: /RawMaterialIssue/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Record deleted successfully."
            });
        }
    }
}
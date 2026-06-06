// Controllers/ProductionFGIssueController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class ProductionFGIssueController : Controller
    {
        private readonly ProductionFGIssueService _svc;
        private readonly ItemSizeService _sizeSvc;

        public ProductionFGIssueController(
            ProductionFGIssueService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        public IActionResult ProductionFGIssueView() => View();

        // GET: /ProductionFGIssue/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /ProductionFGIssue/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /ProductionFGIssue/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDept(deptId));

        // POST: /ProductionFGIssue/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] ProductionFGIssueModel model)
        {
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
                message = "FG Issue updated successfully."
            });
        }

        // POST: /ProductionFGIssue/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "FG Issue record deleted."
            });
        }
    }
}
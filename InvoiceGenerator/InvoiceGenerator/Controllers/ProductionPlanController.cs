// Controllers/ProductionPlanController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class ProductionPlanController : Controller
    {
        private readonly ProductionPlanService _svc;
        private readonly ItemSizeService _sizeSvc;

        public ProductionPlanController(
            ProductionPlanService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /ProductionPlan
        public IActionResult ProductionPlanView() 
            => View();

        // GET: /ProductionPlan/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /ProductionPlan/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /ProductionPlan/GetMachinesByDept?deptId=1
        [HttpGet]
        public IActionResult GetMachinesByDept(int deptId)
            => Json(_svc.GetMachinesByDept(deptId));

        // GET: /ProductionPlan/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDept(deptId));

        // GET: /ProductionPlan/GetSizesByDeptItem?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetSizesByDeptItem(int deptId, int itemId)
            => Json(_svc.GetSizesByDeptItem(deptId, itemId));

        // GET: /ProductionPlan/GetItemDescription?itemId=1
        [HttpGet]
        public IActionResult GetItemDescription(int itemId)
        {
            var result = _svc.GetItemDescription(itemId);
            return result == null ? NotFound() : Json(result);
        }

        // POST: /ProductionPlan/GetRMStockInfo
        [HttpPost]
        public IActionResult GetRMStockInfo(
            [FromBody] RMStockRequestModel req)
            => Json(_svc.GetRMStockInfo(
                req.DepartmentId, req.ItemId, req.SizeId));

        // GET: /ProductionPlan/GetPOScheduleQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetPOScheduleQty(int deptId, int itemId)
            => Json(new
            {
                poSchedule_Qty = _svc.GetPOScheduleQty(deptId, itemId)
            });

        // GET: /ProductionPlan/GetAddedPlanQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetAddedPlanQty(int deptId, int itemId)
            => Json(new
            {
                addedPlan_Qty = _svc.GetAddedPlanQty(deptId, itemId)
            });

        // GET: /ProductionPlan/GetFGQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetFGQty(int deptId, int itemId)
            => Json(new { fG_Qty = _svc.GetFGQty(deptId, itemId) });

        // POST: /ProductionPlan/ValidateTime
        [HttpPost]
        public IActionResult ValidateTime(
            [FromBody] ValidateTimeRequestModel req)
        {
            var hours = _svc.ValidateTime(
                req.DepartmentId, req.MachineId,
                req.ItemId, req.Plan_Qty, req.Plan_Date);
            return Json(new
            {
                required_FinalTime_InHrs = hours
            });
        }

        // POST: /ProductionPlan/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] ProductionPlanModel model)
        {
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
            if (model.SizeId == 0)
                return Json(new
                {
                    success = false,
                    message = "Please select a Size."
                });
            if (model.Plan_Qty <= 0)
                return Json(new
                {
                    success = false,
                    message = "Plan Qty must be greater than 0."
                });
            if (string.IsNullOrEmpty(model.Plan_Date))
                return Json(new
                {
                    success = false,
                    message = "Plan Date is required."
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
                message = "Production Plan updated successfully."
            });
        }

        // POST: /ProductionPlan/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Production Plan deleted."
            });
        }
    }

    // ── Request models for POST bodies ──
    public class RMStockRequestModel
    {
        public int DepartmentId { get; set; }
        public int ItemId { get; set; }
        public int SizeId { get; set; }
    }

    public class ValidateTimeRequestModel
    {
        public int DepartmentId { get; set; }
        public int MachineId { get; set; }
        public int ItemId { get; set; }
        public int Plan_Qty { get; set; }
        public string Plan_Date { get; set; } = "";
    }
}
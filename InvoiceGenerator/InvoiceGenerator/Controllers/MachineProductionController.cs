// Controllers/MachineProductionController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class MachineProductionController : Controller
    {
        private readonly MachineProductionService _svc;
        private readonly ItemSizeService _sizeSvc;

        public MachineProductionController(
            MachineProductionService svc,
            ItemSizeService sizeSvc)
        {
            _svc = svc;
            _sizeSvc = sizeSvc;
        }

        // GET: /MachineProduction
        public IActionResult MachineProductionView() => View();

        // GET: /MachineProduction/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /MachineProduction/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_sizeSvc.GetDepartments());

        // GET: /MachineProduction/GetShifts
        [HttpGet]
        public IActionResult GetShifts()
            => Json(_svc.GetShifts());

        // GET: /MachineProduction/GetMachinesByDept?deptId=1
        [HttpGet]
        public IActionResult GetMachinesByDept(int deptId)
            => Json(_svc.GetMachinesByDept(deptId));

        // GET: /MachineProduction/GetItemsByDept?deptId=1
        [HttpGet]
        public IActionResult GetItemsByDept(int deptId)
            => Json(_svc.GetItemsByDept(deptId));

        // GET: /MachineProduction/GetItemDescription?itemId=1&deptId=1
        [HttpGet]
        public IActionResult GetItemDescription(int itemId, int deptId)
        {
            var result = _svc.GetItemDescription(itemId, deptId);
            return result == null ? NotFound() : Json(result);
        }

        // GET: /MachineProduction/GetPlanQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetPlanQty(int deptId, int itemId)
            => Json(new
            {
                addedPlan_Qty = _svc.GetAddedPlanQty(deptId, itemId)
            });

        // GET: /MachineProduction/GetProduceQty?deptId=1&itemId=2
        [HttpGet]
        public IActionResult GetProduceQty(int deptId, int itemId)
            => Json(new
            {
                produce_Qty = _svc.GetProduceQty(deptId, itemId)
            });

        // GET: /MachineProduction/GetProduceQtyPPBox?deptId=4&itemId=2&machineId=3
        [HttpGet]
        public IActionResult GetProduceQtyPPBox(
            int deptId, int itemId, int machineId)
            => Json(new
            {
                produce_Qty = _svc.GetProduceQtyPPBox(
                    deptId, itemId, machineId)
            });

        // POST: /MachineProduction/Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Save([FromForm] MachineProductionModel model)
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
            if (model.Quantity <= 0)
                return Json(new
                {
                    success = false,
                    message = "Quantity must be greater than 0."
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
                message = "Production data updated successfully."
            });
        }

        // POST: /MachineProduction/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int srNo)
        {
            _svc.Delete(srNo);
            return Json(new
            {
                success = true,
                message = "Production record deleted."
            });
        }
    }
}
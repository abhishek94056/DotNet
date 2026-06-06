// Controllers/NPDPPBoxItemController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class PPBoxNPDItemController : Controller
    {
        private readonly NPDPPBoxItemService _svc;
        //private readonly ItemSizeService _sizeSvc;

        public PPBoxNPDItemController(
            NPDPPBoxItemService svc // ItemSizeService sizeSvc
           )
        {
            _svc = svc;
            //_sizeSvc = sizeSvc;
        }

        // GET: /NPDPPBoxItem
        public IActionResult PPBoxNPDItemView() => View();

        // GET: /NPDPPBoxItem/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /NPDPPBoxItem/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_svc.GetDepartments());

        // POST: /NPDPPBoxItem/Save
        [HttpPost]
        public async Task<IActionResult> Save(
            [FromForm] NPDPPBoxItemModel model,
            IFormFile? file1, IFormFile? file2,
            IFormFile? file3, IFormFile? file4, IFormFile? file5)
        {
            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            if (file1 != null)
                model.Document_File_Name1 = await _svc.SaveFile(file1);
            if (file2 != null)
                model.Document_File_Name2 = await _svc.SaveFile(file2);
            if (file3 != null)
                model.Document_File_Name3 = await _svc.SaveFile(file3);
            if (file4 != null)
                model.Document_File_Name4 = await _svc.SaveFile(file4);
            if (file5 != null)
                model.Document_File_Name5 = await _svc.SaveFile(file5);

            if (model.NPD_ItemId == 0)
                _svc.Insert(model, createdBy);
            else
                _svc.Update(model, createdBy);

            return Json(new
            {
                success = true,
                message = model.NPD_ItemId == 0
                    ? "PP Box NPD Item added successfully."
                    : "PP Box NPD Item updated successfully."
            });
        }

        // POST: /NPDPPBoxItem/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "PP Box NPD Item deleted."
            });
        }
    }
}
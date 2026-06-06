// Controllers/NPDItemController.cs
using InvoiceGenerator.Filters;
using InvoiceGenerator.Helper;
using InvoiceGenerator.Models;
using InvoiceGenerator.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoiceGenerator.Controllers
{
    [RequireLogin]
    public class NPDItemController : Controller
    {
        private readonly NPDItemService _svc;
        //private readonly ItemSizeService _sizeSvc;

        public NPDItemController(
            NPDItemService svc)//,ItemSizeService sizeSvc
        {
            _svc = svc;
            //_sizeSvc = sizeSvc;
        }

        // GET: /NPDItem
        public IActionResult NPDItemView() => View();

        // GET: /NPDItem/GetAll?deptId=1
        [HttpGet]
        public IActionResult GetAll(int deptId)
            => Json(_svc.GetAll(deptId));

        // GET: /NPDItem/GetDepartments
        [HttpGet]
        public IActionResult GetDepartments()
            => Json(_svc.GetDepartments());

        // GET: /NPDItem/GetMachinesByDept?deptId=1
        [HttpGet]
        public IActionResult GetMachinesByDept(int deptId)
            => Json(_svc.GetMachines().Where(m => m.DepartmentId == deptId));
        //{
        //    // Reuse ItemDescriptionService machines filtered by dept
        //    using var con = new Microsoft.Data.SqlClient.SqlConnection(
        //        HttpContext.RequestServices
        //            .GetRequiredService<IConfiguration>()
        //            .GetConnectionString("InvoiceGenerator"));

        //    var list = new List<object>();
        //    using var cmd = new Microsoft.Data.SqlClient.SqlCommand(
        //        "SELECT MachineId, MachineName FROM MachineMaster " +
        //        "WHERE DepartmentId = @d ORDER BY MachineName", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            machineId = Convert.ToInt32(dr["MachineId"]),
        //            machineName = dr["MachineName"].ToString()
        //        });
        //    return Json(list);
        //}

        // POST: /NPDItem/Save  (multipart/form-data for file uploads)
        [HttpPost]
        public async Task<IActionResult> Save(
            [FromForm] NPDItemModel model,
            IFormFile? file1, IFormFile? file2,
            IFormFile? file3, IFormFile? file4, IFormFile? file5)
        {
            string createdBy =
                SessionHelper.GetUserName(HttpContext.Session);

            // Save uploaded files
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
                    ? "NPD Item added successfully."
                    : "NPD Item updated successfully."
            });
        }

        //// POST: /NPDItem/ReUpload
        //[HttpPost]
        //public async Task<IActionResult> ReUpload(
        //    int npdItemId, int departmentId,
        //    IFormFile? file1, IFormFile? file2,
        //    IFormFile? file3, IFormFile? file4, IFormFile? file5)
        //{
        //    string createdBy =
        //        SessionHelper.GetUserName(HttpContext.Session);

        //    string f1 = file1 != null ? await _svc.SaveFile(file1) : "";
        //    string f2 = file2 != null ? await _svc.SaveFile(file2) : "";
        //    string f3 = file3 != null ? await _svc.SaveFile(file3) : "";
        //    string f4 = file4 != null ? await _svc.SaveFile(file4) : "";
        //    string f5 = file5 != null ? await _svc.SaveFile(file5) : "";

        //    _svc.ReUpload(npdItemId, departmentId,
        //        f1, f2, f3, f4, f5, createdBy);

        //    return Json(new
        //    {
        //        success = true,
        //        message = "Documents re-uploaded successfully."
        //    });
        //}

        // POST: /NPDItem/Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            _svc.Delete(id);
            return Json(new
            {
                success = true,
                message = "NPD Item deleted."
            });
        }
    }
}
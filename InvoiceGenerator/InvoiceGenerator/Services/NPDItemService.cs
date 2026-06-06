// Services/NPDItemService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class NPDItemService
    {
        private readonly string _conn;
        private readonly IWebHostEnvironment _env;

        public NPDItemService(IConfiguration config, IWebHostEnvironment env)
        {
            _conn = config.GetConnectionString("InvoiceGenerator")!;
            _env = env;
        }

        // ── SELECT ALL by DepartmentId ──
        public List<NPDItemModel> GetAll(int departmentId)
        {
            var list = new List<NPDItemModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_NPD_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── INSERT ──
        public void Insert(NPDItemModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── UPDATE ──
        public void Update(NPDItemModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Update", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }
        //GetDepartments
        public List<DepartmentModel> GetDepartments()
        {
            var list = new List<DepartmentModel>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

            con.Open();

            using var dr = cmd.ExecuteReader();

            //while (dr.Read())
            //{
            //    list.Add(new DepartmentModel
            //    {
            //        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
            //        Department = dr["Department"]?.ToString() ?? ""
            //    });
            //}
            while (dr.Read())
            {
                int deptId = Convert.ToInt32(dr["DepartmentId"]);
                string deptName = dr["Department"]?.ToString() ?? "";

                // Hide PP Box
                if (deptId == 4 || deptName.Equals("PP Box", StringComparison.OrdinalIgnoreCase))
                    continue;

                list.Add(new DepartmentModel
                {
                    DepartmentId = deptId,
                    Department = deptName
                });
            }
            return list;
        }

        //GetMachines
        public List<MachineDescriptionModel> GetMachines()
        {
            var list = new List<MachineDescriptionModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Machine");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new MachineDescriptionModel
                {
                    MachineId = Convert.ToInt32(dr["MachineId"]),
                    MachineName = dr["MachineName"].ToString()!,
                    DepartmentId = dr["DepartmentId"] == DBNull.Value
                        ? 0 : Convert.ToInt32(dr["DepartmentId"])
                });
            return list;
        }

        //// ── RE-UPLOAD documents only ──
        //public void ReUpload(int npdItemId, int departmentId,
        //    string f1, string f2, string f3, string f4, string f5,
        //    string createdBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_NPD_ITEM_MASTER_SET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "ReUpload");
        //    cmd.Parameters.AddWithValue("@NPD_ItemId", npdItemId);
        //    cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
        //    cmd.Parameters.AddWithValue("@Document_File_Name1", f1 ?? "");
        //    cmd.Parameters.AddWithValue("@Document_File_Name2", f2 ?? "");
        //    cmd.Parameters.AddWithValue("@Document_File_Name3", f3 ?? "");
        //    cmd.Parameters.AddWithValue("@Document_File_Name4", f4 ?? "");
        //    cmd.Parameters.AddWithValue("@Document_File_Name5", f5 ?? "");
        //    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //}

        // ── SOFT DELETE (IsActive = 1) ──
        public void Delete(int npdItemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_NPD_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@NPD_ItemId", npdItemId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── SAVE UPLOADED FILES to wwwroot/uploads/npd ──
        public async Task<string> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return "";

            string uploadFolder = Path.Combine(
                _env.WebRootPath, "uploads", "npd");
            Directory.CreateDirectory(uploadFolder);

            // Unique file name to avoid collision
            string uniqueName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            string filePath = Path.Combine(uploadFolder, uniqueName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return uniqueName;
        }

        // ── SHARED CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            NPDItemModel m, string createdBy)
        {
            var cmd = new SqlCommand("SP_NPD_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@NPD_ItemId", m.NPD_ItemId);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@Date", m.Date);
            cmd.Parameters.AddWithValue("@Marketing_Person", m.Marketing_Person ?? "");
            cmd.Parameters.AddWithValue("@Item_Name", m.Item_Name ?? "");
            cmd.Parameters.AddWithValue("@Item_Code", m.Item_Code ?? "");
            cmd.Parameters.AddWithValue("@Customer_Name", m.Customer_Name ?? "");
            cmd.Parameters.AddWithValue("@Customer_Contact_Person", m.Customer_Contact_Person ?? "");
            cmd.Parameters.AddWithValue("@Customer_Contact_Details", m.Customer_Contact_Details ?? "");
            cmd.Parameters.AddWithValue("@Component_Name", m.Component_Name ?? "");
            cmd.Parameters.AddWithValue("@Specifications", m.Specifications ?? "");
            cmd.Parameters.AddWithValue("@Material_Type", m.Material_Type ?? "");
            cmd.Parameters.AddWithValue("@Shot_Weight", m.Shot_Weight);
            cmd.Parameters.AddWithValue("@Material_Color", m.Material_Color ?? "");
            cmd.Parameters.AddWithValue("@Production_Person", m.Production_Person ?? "");
            cmd.Parameters.AddWithValue("@RM_Size", m.RM_Size ?? "");
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@Document_File_Name1", m.Document_File_Name1 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name2", m.Document_File_Name2 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name3", m.Document_File_Name3 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name4", m.Document_File_Name4 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name5", m.Document_File_Name5 ?? "");
            cmd.Parameters.AddWithValue("@Printing_Matter", m.Printing_Matter ?? "");
            cmd.Parameters.AddWithValue("@Cutting", m.Cutting ?? "");
            cmd.Parameters.AddWithValue("@Packing_Details", m.Packing_Details ?? "");
            cmd.Parameters.AddWithValue("@Delivery_Location", m.Delivery_Location ?? "");
            cmd.Parameters.AddWithValue("@Quality_Person", m.Quality_Person ?? "");
            cmd.Parameters.AddWithValue("@SIR_DateTime", m.SIR_DateTime ?? "");
            cmd.Parameters.AddWithValue("@SIR_Remark", m.SIR_Remark ?? "");
            cmd.Parameters.AddWithValue("@Component_Fitting", m.Component_Fitting ?? "");
            cmd.Parameters.AddWithValue("@Transport_Delivery_Terms", m.Transport_Delivery_Terms ?? "");
            cmd.Parameters.AddWithValue("@Payment_Terms", m.Payment_Terms ?? "");
            cmd.Parameters.AddWithValue("@Rate_of_Product", m.Rate_of_Product);
            cmd.Parameters.AddWithValue("@Rework_Complaint_Details", m.Rework_Complaint_Details ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            return cmd;
        }

        private NPDItemModel Map(SqlDataReader dr)
        {
            T Val<T>(string col, T def = default!)
            {
                try
                {
                    var v = dr[col];
                    return v == DBNull.Value ? def : (T)Convert.ChangeType(v, typeof(T));
                }
                catch { return def; }
            }

            return new NPDItemModel
            {
                NPD_ItemId = Val<int>("NPD_ItemId"),
                DepartmentId = Val<int>("DepartmentId"),
                DepartmentName = Val<string>("Department", ""),
                Date = Val<string>("Date", ""),
                Marketing_Person = Val<string>("Marketing_Person", ""),
                Item_Name = Val<string>("Item_Name", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Customer_Name = Val<string>("Customer_Name", ""),
                Customer_Contact_Person = Val<string>("Customer_Contact_Person", ""),
                Customer_Contact_Details = Val<string>("Customer_Contact_Details", ""),
                Component_Name = Val<string>("Component_Name", ""),
                Specifications = Val<string>("Specifications", ""),
                Material_Type = Val<string>("Material_Type", ""),
                Shot_Weight = Val<decimal>("Shot_Weight"),
                Material_Color = Val<string>("Material_Color", ""),
                Production_Person = Val<string>("Production_Person", ""),
                RM_Size = Val<string>("RM_Size", ""),
                MachineId = Val<int>("MachineId"),
                MachineName = Val<string>("MachineName", ""),
                Document_File_Name1 = Val<string>("Document_File_Name1", ""),
                Document_File_Name2 = Val<string>("Document_File_Name2", ""),
                Document_File_Name3 = Val<string>("Document_File_Name3", ""),
                Document_File_Name4 = Val<string>("Document_File_Name4", ""),
                Document_File_Name5 = Val<string>("Document_File_Name5", ""),
                Printing_Matter = Val<string>("Printing_Matter", ""),
                Cutting = Val<string>("Cutting", ""),
                Packing_Details = Val<string>("Packing_Details", ""),
                Delivery_Location = Val<string>("Delivery_Location", ""),
                Quality_Person = Val<string>("Quality_Person", ""),
                SIR_DateTime = Val<string>("SIR_DateTime", ""),
                SIR_Remark = Val<string>("SIR_Remark", ""),
                Component_Fitting = Val<string>("Component_Fitting", ""),
                Transport_Delivery_Terms = Val<string>("Transport_Delivery_Terms", ""),
                Payment_Terms = Val<string>("Payment_Terms", ""),
                Rate_of_Product = Val<decimal>("Rate_of_Product"),
                Rework_Complaint_Details = Val<string>("Rework_Complaint_Details", ""),
                IsActive = Val<int>("IsActive"),
                Date_Time = Val<string>("Date_Time", ""),
                CreatedBy = Val<string>("CreatedBy", "")
            };
        }
    }
}
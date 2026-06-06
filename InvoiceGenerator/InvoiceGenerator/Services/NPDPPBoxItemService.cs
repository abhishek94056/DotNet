// Services/NPDPPBoxItemService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class NPDPPBoxItemService
    {
        private readonly string _conn;
        private readonly IWebHostEnvironment _env;

        public NPDPPBoxItemService(
            IConfiguration config,
            IWebHostEnvironment env)
        {
            _conn = config.GetConnectionString("InvoiceGenerator")!;
            _env = env;
        }
        //GetDepartments
        //public List<DepartmentModel> GetDepartments()
        //{
        //    var list = new List<DepartmentModel>();

        //    using var con = new SqlConnection(_conn);

        //    using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

        //    con.Open();

        //    using var dr = cmd.ExecuteReader();

        //    //while (dr.Read())
        //    //{
        //    //    list.Add(new DepartmentModel
        //    //    {
        //    //        DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
        //    //        Department = dr["Department"]?.ToString() ?? ""
        //    //    });
        //    //}
        //    while (dr.Read())
        //    {
        //        int deptId = Convert.ToInt32(dr["DepartmentId"]);
        //        string deptName = dr["Department"]?.ToString() ?? "";

        //        // Hide PP Box
        //        if (deptId == 4 || deptName.Equals("PP Box", StringComparison.OrdinalIgnoreCase))
        //            continue;

        //        list.Add(new DepartmentModel
        //        {
        //            DepartmentId = deptId,
        //            Department = deptName
        //        });
        //    }
        //    return list;
        //}
        public List<DepartmentModel> GetDepartments()
        {
            var list = new List<DepartmentModel>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                int deptId = Convert.ToInt32(dr["DepartmentId"]);
                string deptName = dr["Department"]?.ToString() ?? "";

                // Show ONLY PP Box
                if (deptId != 4 &&
                    !deptName.Equals("PP Box", StringComparison.OrdinalIgnoreCase))
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
        //public List<MachineDescriptionModel> GetMachines()
        //{
        //    var list = new List<MachineDescriptionModel>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_Machine");
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new MachineDescriptionModel
        //        {
        //            MachineId = Convert.ToInt32(dr["MachineId"]),
        //            MachineName = dr["MachineName"].ToString()!,
        //            DepartmentId = dr["DepartmentId"] == DBNull.Value
        //                ? 0 : Convert.ToInt32(dr["DepartmentId"])
        //        });
        //    return list;
        //}
        // ── GET ALL ──
        public List<NPDPPBoxItemModel> GetAll(int departmentId)
        {
            var list = new List<NPDPPBoxItemModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_NPD_PPBOX_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── INSERT ──
        public void Insert(NPDPPBoxItemModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── UPDATE ──
        public void Update(NPDPPBoxItemModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Update", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }


        // ── SOFT DELETE ──
        public void Delete(int npdItemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_NPD_PPBOX_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@NPD_ItemId", npdItemId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── SAVE FILE ──
        public async Task<string> SaveFile(IFormFile file)
        {
            if (file == null || file.Length == 0) return "";
            string folder = Path.Combine(
                _env.WebRootPath, "uploads", "npd_ppbox");
            Directory.CreateDirectory(folder);
            string uniqueName =
                $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            using var stream = new FileStream(
                Path.Combine(folder, uniqueName), FileMode.Create);
            await file.CopyToAsync(stream);
            return uniqueName;
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            NPDPPBoxItemModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_NPD_PPBOX_ITEM_MASTER_SET", con)
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
            cmd.Parameters.AddWithValue("@GSM_of_Box", m.GSM_of_Box ?? "");
            cmd.Parameters.AddWithValue("@GSM_of_Partition", m.GSM_of_Partition ?? "");
            cmd.Parameters.AddWithValue("@Color_of_Box", m.Color_of_Box ?? "");
            cmd.Parameters.AddWithValue("@Production_Person", m.Production_Person ?? "");
            cmd.Parameters.AddWithValue("@Sheet_Size", m.Sheet_Size ?? "");
            cmd.Parameters.AddWithValue("@Sheet_Size_Partition", m.Sheet_Size_Partition ?? "");
            cmd.Parameters.AddWithValue("@Material_Color", m.Material_Color ?? "");
            cmd.Parameters.AddWithValue("@Flap", m.Flap ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name1", m.Document_File_Name1 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name2", m.Document_File_Name2 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name3", m.Document_File_Name3 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name4", m.Document_File_Name4 ?? "");
            cmd.Parameters.AddWithValue("@Document_File_Name5", m.Document_File_Name5 ?? "");
            cmd.Parameters.AddWithValue("@Support_Party", m.Support_Party ?? "");
            cmd.Parameters.AddWithValue("@Handle_Material_Grade", m.Handle_Material_Grade ?? "");
            cmd.Parameters.AddWithValue("@Handle_Fixing_Method", m.Handle_Fixing_Method ?? "");
            cmd.Parameters.AddWithValue("@Cloth", m.Cloth ?? "");
            cmd.Parameters.AddWithValue("@Printing_Matter", m.Printing_Matter ?? "");
            cmd.Parameters.AddWithValue("@Cutting", m.Cutting ?? "");
            cmd.Parameters.AddWithValue("@Packing_Details", m.Packing_Details ?? "");
            cmd.Parameters.AddWithValue("@Delivery_Location", m.Delivery_Location ?? "");
            cmd.Parameters.AddWithValue("@Quality_Person", m.Quality_Person ?? "");
            cmd.Parameters.AddWithValue("@SIR_DateTime", m.SIR_DateTime ?? "");
            cmd.Parameters.AddWithValue("@SIR_Remark", m.SIR_Remark ?? "");
            cmd.Parameters.AddWithValue("@Transport_Delivery_Terms", m.Transport_Delivery_Terms ?? "");
            cmd.Parameters.AddWithValue("@Payment_Terms", m.Payment_Terms ?? "");
            cmd.Parameters.AddWithValue("@Rate_of_Product", m.Rate_of_Product);
            cmd.Parameters.AddWithValue("@Rework_Complaint_Details", m.Rework_Complaint_Details ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            return cmd;
        }

        private NPDPPBoxItemModel Map(SqlDataReader dr)
        {
            T Val<T>(string col, T def = default!)
            {
                try
                {
                    var v = dr[col];
                    return v == DBNull.Value
                        ? def : (T)Convert.ChangeType(v, typeof(T));
                }
                catch { return def; }
            }

            return new NPDPPBoxItemModel
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
                GSM_of_Box = Val<string>("GSM_of_Box", ""),
                GSM_of_Partition = Val<string>("GSM_of_Partition", ""),
                Color_of_Box = Val<string>("Color_of_Box", ""),
                Production_Person = Val<string>("Production_Person", ""),
                Sheet_Size = Val<string>("Sheet_Size", ""),
                Sheet_Size_Partition = Val<string>("Sheet_Size_Partition", ""),
                Material_Color = Val<string>("Material_Color", ""),
                Flap = Val<string>("Flap", ""),
                Document_File_Name1 = Val<string>("Document_File_Name1", ""),
                Document_File_Name2 = Val<string>("Document_File_Name2", ""),
                Document_File_Name3 = Val<string>("Document_File_Name3", ""),
                Document_File_Name4 = Val<string>("Document_File_Name4", ""),
                Document_File_Name5 = Val<string>("Document_File_Name5", ""),
                Support_Party = Val<string>("Support_Party", ""),
                Handle_Material_Grade = Val<string>("Handle_Material_Grade", ""),
                Handle_Fixing_Method = Val<string>("Handle_Fixing_Method", ""),
                Cloth = Val<string>("Cloth", ""),
                Printing_Matter = Val<string>("Printing_Matter", ""),
                Cutting = Val<string>("Cutting", ""),
                Packing_Details = Val<string>("Packing_Details", ""),
                Delivery_Location = Val<string>("Delivery_Location", ""),
                Quality_Person = Val<string>("Quality_Person", ""),
                SIR_DateTime = Val<string>("SIR_DateTime", ""),
                SIR_Remark = Val<string>("SIR_Remark", ""),
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
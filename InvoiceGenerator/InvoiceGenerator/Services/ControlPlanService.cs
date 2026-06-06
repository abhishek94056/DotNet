// Services/ControlPlanService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ControlPlanService
    {
        private readonly string _conn;

        public ControlPlanService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (DeptId=1 only, IsActive=0) ──
        public List<ControlPlanModel> GetAll(int deptId)
        {
            var list = new List<ControlPlanModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_Add_Control_Plan_Item_Wise_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        //// ── GET ITEMS BY DEPT ──
        //public List<object> GetItemsByDept(int deptId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT ItemId, Item_Code, Item_Description " +
        //        "FROM Item_Description_Master " +
        //        "WHERE DepartmentId = @d ORDER BY Item_Code", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            itemId = Convert.ToInt32(dr["ItemId"]),
        //            item_Code = dr["Item_Code"].ToString(),
        //            item_Description = dr["Item_Description"].ToString()
        //        });
        //    return list;
        //}
        public List<object> GetItemsByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    itemId = Convert.ToInt32(dr["ItemId"]),
                    item_Code = dr["Item_Code"].ToString(),
                    item_Description = dr["Item_Description"].ToString()
                });
            }

            return list;
        }
        // ── INSERT ──
        public (bool success, string message) Insert(
            ControlPlanModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Control Plan saved successfully.");
        }

        // ── UPDATE ──
        public void Update(ControlPlanModel m, string updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Update", con, m, updatedBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── SOFT DELETE ──
        public void Delete(int srNo)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_Add_Control_Plan_Item_Wise_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            ControlPlanModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_Add_Control_Plan_Item_Wise_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Pressure_Time", m.Pressure_Time ?? "");
            cmd.Parameters.AddWithValue("@Punching_Pressure", m.Punching_Pressure ?? "");
            cmd.Parameters.AddWithValue("@Set_mm", m.Set_mm ?? "");
            cmd.Parameters.AddWithValue("@Cycle_Delay_Time", m.Cycle_Delay_Time ?? "");
            cmd.Parameters.AddWithValue("@Vaccum_Time", m.Vaccum_Time ?? "");
            cmd.Parameters.AddWithValue("@Cooling_Time", m.Cooling_Time ?? "");
            cmd.Parameters.AddWithValue("@Ejection_Time", m.Ejection_Time ?? "");
            cmd.Parameters.AddWithValue("@Winder_Time", m.Winder_Time ?? "");
            for (int z = 1; z <= 16; z++)
            {
                var val = typeof(ControlPlanModel)
                    .GetProperty($"Zone_{z}")!
                    .GetValue(m) as string ?? "";
                cmd.Parameters.AddWithValue($"@Zone_{z}", val);
            }
            cmd.Parameters.AddWithValue("@Packing_Details", m.Packing_Details ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            return cmd;
        }

        private ControlPlanModel Map(SqlDataReader dr)
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

            var m = new ControlPlanModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                ItemId = Val<int>("ItemId"),
                Pressure_Time = Val<string>("Pressure_Time", ""),
                Punching_Pressure = Val<string>("Punching_Pressure", ""),
                Set_mm = Val<string>("Set_mm", ""),
                Cycle_Delay_Time = Val<string>("Cycle_Delay_Time", ""),
                Vaccum_Time = Val<string>("Vaccum_Time", ""),
                Cooling_Time = Val<string>("Cooling_Time", ""),
                Ejection_Time = Val<string>("Ejection_Time", ""),
                Winder_Time = Val<string>("Winder_Time", ""),
                Packing_Details = Val<string>("Packing_Details", ""),
                CreatedBy = Val<string>("CreatedBy", ""),
                IsActive = Val<int>("IsActive"),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Department = Val<string>("Department", ""),
                Add_Date = Val<string>("Add_Date", "")
            };

            for (int z = 1; z <= 16; z++)
                typeof(ControlPlanModel)
                    .GetProperty($"Zone_{z}")!
                    .SetValue(m, Val<string>($"Zone_{z}", ""));

            return m;
        }
    }
}
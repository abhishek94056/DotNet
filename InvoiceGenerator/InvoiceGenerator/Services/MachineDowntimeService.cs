// Services/MachineDowntimeService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class MachineDowntimeService
    {
        private readonly string _conn;

        public MachineDowntimeService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (dept + month) ──
        public List<MachineDowntimeModel> GetAll(int deptId, int monthId)
        {
            var list = new List<MachineDowntimeModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_DOWNTIME_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        //// ── GET SHIFTS ──
        //public List<object> GetShifts()
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT ShiftId, ShiftName " +
        //        "FROM Shift_Master ORDER BY ShiftId", con);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            shiftId = Convert.ToInt32(dr["ShiftId"]),
        //            shiftName = dr["ShiftName"].ToString()
        //        });
        //    return list;
        //}
        public List<object> GetShifts()
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Shift");

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    shiftId = Convert.ToInt32(dr["ShiftId"]),
                    shiftName = dr["ShiftName"].ToString()
                });
            }

            return list;
        }
        //// ── GET MACHINES BY DEPT ──
        //public List<object> GetMachinesByDept(int deptId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
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
        //    return list;
        //}
        public List<object> GetMachinesByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_MachineByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    machineId = Convert.ToInt32(dr["MachineId"]),
                    machineName = dr["MachineName"].ToString()
                });
            }

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

        //public List<object> GetItemsByDept(int deptId)
        //{
        //    var list = new List<object>();

        //    using var con = new SqlConnection(_conn);

        //    using var cmd = new SqlCommand(
        //        "SP_DROP_DOWN_MASTER_GET", con);

        //    cmd.CommandType = CommandType.StoredProcedure;

        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDept");
        //    cmd.Parameters.AddWithValue("@DepartmentId", deptId);
        //    cmd.Parameters.AddWithValue("@CustomerId", 0);
        //    cmd.Parameters.AddWithValue("@ItemId", 0);

        //    con.Open();

        //    using var dr = cmd.ExecuteReader();

        //    while (dr.Read())
        //    {
        //        list.Add(new
        //        {
        //            itemId = Convert.ToInt32(dr["ItemId"]),
        //            item_Code = dr["Item_Code"].ToString(),
        //            item_Description = dr["Item_Description"].ToString()
        //        });
        //    }

        //    return list;
        //}
        // ── GET DOWNTIME REASONS ──
        public List<object> GetDowntimeReasons()
        {
            // Hardcoded to match SP DownTime_ReasonId 1-10
            return new List<object>
            {
                new { downTime_ReasonId = 1,  downTime_Reason = "No Operator"        },
                new { downTime_ReasonId = 2,  downTime_Reason = "Tool Change"        },
                new { downTime_ReasonId = 3,  downTime_Reason = "No Power"           },
                new { downTime_ReasonId = 4,  downTime_Reason = "Machine Break Down" },
                new { downTime_ReasonId = 5,  downTime_Reason = "No Material"        },
                new { downTime_ReasonId = 6,  downTime_Reason = "MC Setting"         },
                new { downTime_ReasonId = 7,  downTime_Reason = "No Load"            },
                new { downTime_ReasonId = 8,  downTime_Reason = "Training"           },
                new { downTime_ReasonId = 9,  downTime_Reason = "Quality Issue"      },
                new { downTime_ReasonId = 10, downTime_Reason = "Actual Production"  }
            };
        }

        // ── INSERT ──
        public (bool success, string message) Insert(
            MachineDowntimeModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Downtime record saved successfully.");
        }

        // ── UPDATE ──
        public void Update(MachineDowntimeModel m, string updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Update", con, m, updatedBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── DELETE ──
        public void Delete(int srNo)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_DOWNTIME_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            MachineDowntimeModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_MACHINE_DOWNTIME_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@ShiftId", m.ShiftId);
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@MC_StatusId", m.MC_StatusId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@DownTime_ReasonId", m.DownTime_ReasonId);
            cmd.Parameters.AddWithValue("@DownTime", m.DownTime);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@MonthId",
                string.IsNullOrEmpty(m.Date)
                    ? DateTime.Now.Month
                    : DateTime.Parse(m.Date).Month);
            return cmd;
        }

        private MachineDowntimeModel Map(SqlDataReader dr)
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
            return new MachineDowntimeModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                ShiftId = Val<int>("ShiftId"),
                MachineId = Val<int>("MachineId"),
                ItemId = Val<int>("ItemId"),
                Department = Val<string>("Department", ""),
                ShiftName = Val<string>("ShiftName", ""),
                MachineName = Val<string>("MachineName", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Add_Date = Val<string>("Add_Date", ""),
                Actual_Production_Hrs = Val<decimal>("Actual_Production_Hrs"),
                No_Operator_InHrs = Val<decimal>("No_Operator_InHrs"),
                Tool_Change_InHrs = Val<decimal>("Tool_Change_InHrs"),
                No_Power_InHrs = Val<decimal>("No_Power_InHrs"),
                Machine_Break_Down_InHrs = Val<decimal>("Machine_Break_Down_InHrs"),
                No_Material_InHrs = Val<decimal>("No_Material_InHrs"),
                MC_Setting_InHrs = Val<decimal>("MC_Setting_InHrs"),
                No_Load_InHrs = Val<decimal>("No_Load_InHrs"),
                Training_InHrs = Val<decimal>("Training_InHrs"),
                QualityIssue_InHrs = Val<decimal>("QualityIssue_InHrs")
            };
        }
    }
}
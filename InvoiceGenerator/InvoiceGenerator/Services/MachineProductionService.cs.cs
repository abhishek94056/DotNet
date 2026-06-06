// Services/MachineProductionService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class MachineProductionService
    {
        private readonly string _conn;

        public MachineProductionService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (current month) ──
        public List<MachineProductionModel> GetAll(int departmentId)
        {
            var list = new List<MachineProductionModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_PRODUCTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
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
        //        "SELECT ShiftId, ShiftName FROM Shift_Master " +
        //        "ORDER BY ShiftId", con);
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
        // ── GET SHIFTS USING STORED PROCEDURE ──
        public List<object> GetShifts()
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_Shift");
            cmd.Parameters.AddWithValue("@DepartmentId", 0);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

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
        // ── GET MACHINES BY DEPT USING STORED PROCEDURE ──
        public List<object> GetMachinesByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_MachineByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

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
        // ── GET ITEMS BY DEPT ──
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
        // ── GET ITEMS BY DEPT USING STORED PROCEDURE ──
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


        // ── GET ITEM DESCRIPTION ──
        public object? GetItemDescription(int itemId, int deptId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SELECT Item_Description FROM Item_Description_Master " +
                "WHERE ItemId = @i AND DepartmentId = @d", con);
            cmd.Parameters.AddWithValue("@i", itemId);
            cmd.Parameters.AddWithValue("@d", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new
            {
                item_Description = dr["Item_Description"].ToString()
            };
        }

        // ── GET PLAN QTY (non-PPBox dept) ──
        public int GetAddedPlanQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_AddedPlanQty");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── GET PRODUCE QTY (non-PPBox dept) ──
        public int GetProduceQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_PRODUCTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ProduceQty");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── GET PRODUCE QTY PPBox (dept=4) ──
        public int GetProduceQtyPPBox(int deptId, int itemId, int machineId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_PRODUCTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ProduceQty_PPBox");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            cmd.Parameters.AddWithValue("@MachineId", machineId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── INSERT ──
        //public (bool success, string message) Insert(
        //    MachineProductionModel m, string createdBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = BuildCmd("Insert", con, m, createdBy);
        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //    return (true, "Production data saved successfully.");
        //}
        public (bool success, string message) Insert(
    MachineProductionModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);

            con.Open();

            cmd.ExecuteNonQuery();

            return (true, "Production data saved successfully.");
        }
        // ── UPDATE ──
        public void Update(MachineProductionModel m, string updatedBy)
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
                "SP_MACHINE_PRODUCTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            MachineProductionModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_MACHINE_PRODUCTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@ShiftId", m.ShiftId);
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            return cmd;
        }

        private MachineProductionModel Map(SqlDataReader dr)
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
            return new MachineProductionModel
            {
                SrNo = Val<int>("SrNo"),
                ShiftId = Val<int>("ShiftId"),
                MachineId = Val<int>("MachineId"),
                ItemId = Val<int>("ItemId"),
                Quantity = Val<int>("Quantity"),
                DepartmentId = Val<int>("DepartmentId"),
                SizeId = Val<int>("SizeId"),
                CreatedBy = Val<string>("CreatedBy", ""),
                ShiftName = Val<string>("ShiftName", ""),
                MachineName = Val<string>("MachineName", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                DepartmentName = Val<string>("DepartmentName", ""),
                Add_Date = Val<string>("Add_Date", ""),
                IsActive = Val<int>("IsActive")
            };
        }
    }
}
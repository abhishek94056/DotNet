// Services/MachineRejectionService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class MachineRejectionService
    {
        private readonly string _conn;

        public MachineRejectionService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL ──
        public List<MachineRejectionModel> GetAll(int departmentId)
        {
            var list = new List<MachineRejectionModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MACHINE_REJECTION_DATA_DETAILS_SET", con)
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

        // ── GET SHIFTS ──
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


        // ── GET MACHINES BY DEPT ──
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
        //        "SELECT ItemId, Item_Code, Item_Description, " +
        //        "ISNULL(Std_Shot_Weight, 0) AS Std_Shot_Weight " +
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
        //            item_Description = dr["Item_Description"].ToString(),
        //            std_Shot_Weight = Convert.ToDecimal(dr["Std_Shot_Weight"])
        //        });
        //    return list;
        //}

        // ── GET ITEMS BY DEPT ──
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
                    item_Description = dr["Item_Description"].ToString(),
                    std_Shot_Weight = Convert.ToDecimal(
                        dr["Std_Shot_Weight"] == DBNull.Value ? 0 : dr["Std_Shot_Weight"]
                    )
                });
            }

            return list;
        }

        // ── GET REJECTION TYPES ──
        //public List<object> GetRejectionTypes()
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SP_MACHINE_REJECTION_DATA_DETAILS_SET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_RejectionTypes");
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            rejectionId = Convert.ToInt32(dr["RejectionId"]),
        //            rejection_Type = dr["Rejection_Reason"].ToString()
        //        });
        //    return list;
        //}
        public List<object> GetRejectionTypes(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_RejectionByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    rejectionId = Convert.ToInt32(dr["RejectionId"]),
                    rejection_Type = dr["Rejection_Reason"].ToString()
                });
            }

            return list;
        }
        // ── GET OPERATORS BY DEPT ──
        //public List<object> GetOperatorsByDept(int deptId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SP_MACHINE_REJECTION_DATA_DETAILS_SET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_Operators");
        //    cmd.Parameters.AddWithValue("@DepartmentId", deptId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            operatorId = Convert.ToInt32(dr["OperatorId"]),
        //            operatorName = dr["OperatorName"].ToString()
        //        });
        //    return list;
        //}

        public List<object> GetOperatorsByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Operators");

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    operatorId = Convert.ToInt32(dr["OperatorId"]),
                    operatorName = dr["OperatorName"].ToString()
                });
            }

            return list;
        }

        // ── INSERT ──
        public (bool success, string message) Insert(
            MachineRejectionModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Rejection data saved successfully.");
        }

        // ── UPDATE ──
        public void Update(MachineRejectionModel m, string updatedBy)
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
                "SP_MACHINE_REJECTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            MachineRejectionModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_MACHINE_REJECTION_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@ShiftId", m.ShiftId);
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@OperatorId", m.OperatorId);
            cmd.Parameters.AddWithValue("@RejectionId", m.RejectionId);
            cmd.Parameters.AddWithValue("@Rejection_Qty", m.Rejection_Qty);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@Actual_Shot_Weight", m.Actual_Shot_Weight);
            cmd.Parameters.AddWithValue("@Remark", m.Remark ?? "");
            cmd.Parameters.AddWithValue("@Finish_Weight", m.Finish_Weight);
            return cmd;
        }

        private MachineRejectionModel Map(SqlDataReader dr)
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
            return new MachineRejectionModel
            {
                SrNo = Val<int>("SrNo"),
                ShiftId = Val<int>("ShiftId"),
                MachineId = Val<int>("MachineId"),
                ItemId = Val<int>("ItemId"),
                OperatorId = Val<int>("OperatorId"),
                RejectionId = Val<int>("RejectionId"),
                Rejection_Qty = Val<int>("Rejection_Qty"),
                DepartmentId = Val<int>("DepartmentId"),
                Actual_Shot_Weight = Val<decimal>("Actual_Shot_Weight"),
                Finish_Weight = Val<decimal>("Finish_Weight"),
                CreatedBy = Val<string>("CreatedBy", ""),
                Remark = Val<string>("Remark", ""),
                ShiftName = Val<string>("ShiftName", ""),
                MachineName = Val<string>("MachineName", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                DepartmentName = Val<string>("DepartmentName", ""),
                Rejection_Reason = Val<string>("Rejection_Reason", ""),
                OperatorName = Val<string>("OperatorName", ""),
                Add_Date = Val<string>("Add_Date", "")
            };
        }
    }
}
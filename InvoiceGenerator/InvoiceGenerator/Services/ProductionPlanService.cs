// Services/ProductionPlanService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ProductionPlanService
    {
        private readonly string _conn;

        public ProductionPlanService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL ──
        public List<ProductionPlanModel> GetAll(int departmentId)
        {
            var list = new List<ProductionPlanModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
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

            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con);

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
        // ── GET ITEMS BY DEPT ──
        public List<object> GetItemsByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con);

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

        //// ── GET SIZES BY DEPT + ITEM ──
        //public List<object> GetSizesByDeptItem(int deptId, int itemId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    // Sizes linked to item via Matrix_Item_Size_Master
        //    using var cmd = new SqlCommand(
        //        "SELECT s.SizeId, s.ItemSize_Code, s.Item_Size " +
        //        "FROM ItemSizeMaster s " +
        //        "INNER JOIN Matrix_Item_Size_Master m " +
        //        "    ON m.SizeId = s.SizeId " +
        //        "WHERE m.DepartmentId = @d AND m.ItemId = @i " +
        //        "ORDER BY s.ItemSize_Code", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
        //    cmd.Parameters.AddWithValue("@i", itemId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            sizeId = Convert.ToInt32(dr["SizeId"]),
        //            itemSize_Code = dr["ItemSize_Code"].ToString(),
        //            item_Size = dr["Item_Size"].ToString()
        //        });
        //    return list;
        //}
        // ── GET SIZES BY DEPT + ITEM ──
        public List<object> GetSizesByDeptItem(int deptId, int itemId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_SizeByDeptItem");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", itemId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    sizeId = Convert.ToInt32(dr["SizeId"]),
                    itemSize_Code = dr["ItemSize_Code"].ToString()
                });
            }

            return list;
        }

        // ── GET ITEM DESCRIPTION ──
        public object? GetItemDescription(int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Item_By_ItemCode");
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new
            {
                item_Description = dr["Item_Description"].ToString(),
                std_Shot_Weight = dr["Std_Shot_Weight"] == DBNull.Value
                    ? 0m : Convert.ToDecimal(dr["Std_Shot_Weight"])
            };
        }

        // ── GET RM STOCK BY ITEM + SIZE ──
        public RMStockInfoModel GetRMStockInfo(
            int deptId, int itemId, int sizeId)
        {
            using var con = new SqlConnection(_conn);
            // Stock = total RM qty for this size in this dept
            // Used  = plan_qty * shot_weight / 1000 for this month
            using var cmd = new SqlCommand(@"
                DECLARE @Shot_Weight decimal(8,2) = 0;
                SELECT @Shot_Weight = ISNULL(Std_Shot_Weight, 0)
                FROM Item_Description_Master
                WHERE ItemId = @ItemId;

                DECLARE @Stock decimal(8,2) = 0;
                SELECT @Stock = ISNULL(SUM(Quantity), 0)
                FROM Raw_Material_Master
                WHERE DepartmentId = @DeptId AND SizeId = @SizeId
                  AND IsActive = 0;

                DECLARE @Used decimal(8,2) = 0;
                SELECT @Used = ISNULL(
                    CAST(SUM(Plan_Qty) * @Shot_Weight / 1000.0 AS decimal(8,2)), 0)
                FROM Production_Plan_Master
                WHERE DepartmentId = @DeptId AND SizeId = @SizeId
                  AND IsActive = 0
                  AND MONTH(Date) = MONTH(GETDATE())
                  AND YEAR(Date)  = YEAR(GETDATE());

                SELECT @Stock          AS Stock_RM_Qty,
                       @Used           AS Used_RM_Qty,
                       (@Stock - @Used) AS Available_RM_Qty,
                       @Shot_Weight    AS Shot_Weight;",
                con);
            cmd.Parameters.AddWithValue("@DeptId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            cmd.Parameters.AddWithValue("@SizeId", sizeId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return new RMStockInfoModel();
            return new RMStockInfoModel
            {
                Stock_RM_Qty = dr["Stock_RM_Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Stock_RM_Qty"]),
                Used_RM_Qty = dr["Used_RM_Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Used_RM_Qty"]),
                Available_RM_Qty = dr["Available_RM_Qty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Available_RM_Qty"]),
                Shot_Weight = dr["Shot_Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Shot_Weight"])
            };
        }

        // ── PO SCHEDULE QTY ──
        public int GetPOScheduleQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_POSchedule");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── ADDED PLAN QTY ──
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

        // ── FG QTY ──
        public int GetFGQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemFG");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── VALIDATE TIME ──
        public decimal ValidateTime(
            int deptId, int machineId, int itemId,
            int planQty, string planDate)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "ValidateAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MachineId", machineId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            cmd.Parameters.AddWithValue("@Plan_Qty", planQty);
            cmd.Parameters.AddWithValue("@Plan_Date",
                string.IsNullOrEmpty(planDate)
                    ? (object)DBNull.Value : DateTime.Parse(planDate));
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0m : Convert.ToDecimal(result);
        }

        // ── INSERT ──
        public (bool success, string message) Insert(
            ProductionPlanModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Production Plan saved successfully.");
        }

        // ── UPDATE ──
        public void Update(ProductionPlanModel m, string updatedBy)
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
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            ProductionPlanModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_PRODUCTION_PLAN_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Plan_Qty", m.Plan_Qty);
            cmd.Parameters.AddWithValue("@Plan_Date",
                string.IsNullOrEmpty(m.Plan_Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Plan_Date));
            cmd.Parameters.AddWithValue("@Remark", m.Remark ?? "");
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@Date_Time", DBNull.Value);
            return cmd;
        }

        private ProductionPlanModel Map(SqlDataReader dr)
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
            return new ProductionPlanModel
            {
                SrNo = Val<int>("SrNo"),
                MachineId = Val<int>("MachineId"),
                ItemId = Val<int>("ItemId"),
                Plan_Qty = Val<int>("Plan_Qty"),
                SizeId = Val<int>("SizeId"),
                DepartmentId = Val<int>("DepartmentId"),
                CreatedBy = Val<string>("CreatedBy", ""),
                Remark = Val<string>("Remark", ""),
                MachineName = Val<string>("MachineName", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                DepartmentName = Val<string>("DepartmentName", ""),
                Add_Date = Val<string>("Add_Date", ""),
                ItemSize_Code = Val<string>("ItemSize_Code", ""),
                RM_Required = Val<decimal>("RM_Required"),
                Produce_Qty = Val<int>("Produce_Qty"),
                IsActive = Val<int>("IsActive")
            };
        }
    }
}
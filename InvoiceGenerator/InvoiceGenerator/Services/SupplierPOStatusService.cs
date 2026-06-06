// Services/SupplierPOStatusService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class SupplierPOStatusService
    {
        private readonly string _conn;

        public SupplierPOStatusService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (by dept + size + month + year) ──
        public List<SupplierPOStatusModel> GetAll(
            int deptId, int sizeId, int monthId, int yearId)
        {
            var list = new List<SupplierPOStatusModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_SUPPLIER_PO_STATUS_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@SizeId", sizeId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            cmd.Parameters.AddWithValue("@YearId", yearId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── SELECT ALL MONTH (all sizes, by dept + month + year) ──
        public List<SupplierPOStatusModel> GetAllMonth(
            int deptId, int monthId, int yearId)
        {
            var list = new List<SupplierPOStatusModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_SUPPLIER_PO_STATUS_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Month");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            cmd.Parameters.AddWithValue("@YearId", yearId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── SELECT REPORTING (from reporting table) ──
        public List<SupplierPOStatusModel> GetReporting(
            int deptId, int monthId, int yearId)
        {
            var list = new List<SupplierPOStatusModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_SUPPLIER_PO_STATUS_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Month_Reporting");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            cmd.Parameters.AddWithValue("@YearId", yearId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(MapReporting(dr));
            return list;
        }

        //// ── GET SUPPLIERS ──
        //public List<object> GetSuppliers()
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT SupplierId, Supplier " +
        //        "FROM SupplierMaster ORDER BY Supplier", con);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            supplierId = Convert.ToInt32(dr["SupplierId"]),
        //            supplier = dr["Supplier"].ToString()
        //        });
        //    return list;
        //}
        public List<object> GetSuppliers()
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Supplier");
            cmd.Parameters.AddWithValue("@DepartmentId", 0);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    supplierId = Convert.ToInt32(dr["SupplierId"]),
                    supplier = dr["Supplier"].ToString()
                });
            }

            return list;
        }
        //// ── GET SIZES BY DEPT ──
        //public List<object> GetSizesByDept(int deptId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT SizeId, ItemSize_Code, Item_Size " +
        //        "FROM Item_Size_Master " +
        //        "WHERE DepartmentId = @d AND IsActive = 0 " +
        //        "ORDER BY ItemSize_Code", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
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
        public List<object> GetSizesByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemSizeByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    sizeId = Convert.ToInt32(dr["SizeId"]),
                    itemSize_Code = dr["ItemSize_Code"].ToString(),
                    item_Size = dr["Item_Size"].ToString()
                });
            }

            return list;
        }
        // ── INSERT or UPDATE (SP handles both based on RM_Flag) ──
        public (bool success, string message) Save(
            SupplierPOStatusModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd(
                m.SrNo == 0 ? "Insert" : "Update",
                con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, m.SrNo == 0
                ? "PO Status saved successfully."
                : "PO Status updated successfully.");
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            SupplierPOStatusModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_SUPPLIER_PO_STATUS_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@SupplierId", m.SupplierId);
            cmd.Parameters.AddWithValue("@PO_Number", m.PO_Number ?? "");
            cmd.Parameters.AddWithValue("@PO_Date",
                string.IsNullOrEmpty(m.PO_Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.PO_Date));
            cmd.Parameters.AddWithValue("@Date_of_Requirement",
                string.IsNullOrEmpty(m.Date_of_Requirement)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date_of_Requirement));
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@PO_Rate", m.PO_Rate);
            cmd.Parameters.AddWithValue("@Supplier_Invoice", m.Supplier_Invoice ?? "");
            cmd.Parameters.AddWithValue("@Batch_Number", m.Batch_Number ?? "");
            cmd.Parameters.AddWithValue("@RM_Flag", m.RM_Flag);
            cmd.Parameters.AddWithValue("@RM_Qty", m.RM_Qty);
            cmd.Parameters.AddWithValue("@Date_of_Receipt",
                string.IsNullOrEmpty(m.Date_of_Receipt)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date_of_Receipt));
            cmd.Parameters.AddWithValue("@Remarks", m.Remarks ?? "");
            cmd.Parameters.AddWithValue("@Debit_Amount_to_Supplier",
                m.Debit_Amount_to_Supplier);
            cmd.Parameters.AddWithValue("@Supplier_Rating_Quality",
                m.Supplier_Rating_Quality);
            cmd.Parameters.AddWithValue("@Supplier_Rating_Time",
                m.Supplier_Rating_Time);
            cmd.Parameters.AddWithValue("@Supplier_Rating_Process",
                m.Supplier_Rating_Process);
            cmd.Parameters.AddWithValue("@Rack_No_Storage_Location",
                m.Rack_No_Storage_Location ?? "");
            cmd.Parameters.AddWithValue("@Issue_with_Material",
                m.Issue_with_Material ?? "");
            cmd.Parameters.AddWithValue("@CAPA_Action_Plan_recd_from_Supplier",
                m.CAPA_Action_Plan_recd_from_Supplier ?? "");
            cmd.Parameters.AddWithValue("@Action_Plan_Apporved_by_AE_Quality_Dept",
                m.Action_Plan_Apporved_by_AE_Quality_Dept ?? "");
            cmd.Parameters.AddWithValue("@Action_Plan_for_Supplier",
                m.Action_Plan_for_Supplier ?? "");
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@MonthId", m.MonthId);
            cmd.Parameters.AddWithValue("@YearId", m.YearId);
            return cmd;
        }

        private SupplierPOStatusModel Map(SqlDataReader dr)
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
            return new SupplierPOStatusModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                Department = Val<string>("Department", ""),
                SupplierId = Val<int>("SupplierId"),
                Supplier = Val<string>("Supplier", ""),
                PO_Number = Val<string>("PO_Number", ""),
                PO_Date = Val<string>("PO_Date", ""),
                Date_of_Requirement = Val<string>("Date_of_Requirement", ""),
                SizeId = Val<int>("SizeId"),
                ItemSize_Code = Val<string>("ItemSize_Code", ""),
                Item_Size = Val<string>("Item_Size", ""),
                PO_Quantity = Val<decimal>("PO_Quantity"),
                PO_Rate = Val<decimal>("PO_Rate"),
                Supplier_Invoice = Val<string>("Supplier_Invoice", ""),
                Batch_Number = Val<string>("Batch_Number", ""),
                RM_Receipt_Qty = Val<decimal>("RM_Receipt_Qty"),
                RM_Pro_Issue_Qty = Val<decimal>("RM_Pro_Issue_Qty"),
                RM_Pro_Return_Qty = Val<decimal>("RM_Pro_Return_Qty"),
                RM_Supp_Return_Qty = Val<decimal>("RM_Supp_Return_Qty"),
                RM_FG_Qty = Val<decimal>("RM_FG_Qty"),
                Date_of_Receipt = Val<string>("Date_of_Receipt", ""),
                Remarks = Val<string>("Remarks", ""),
                Debit_Amount_to_Supplier = Val<decimal>("Debit_Amount_to_Supplier"),
                Supplier_Rating_Quality = Val<int>("Supplier_Rating_Quality"),
                Supplier_Rating_Time = Val<int>("Supplier_Rating_Time"),
                Supplier_Rating_Process = Val<int>("Supplier_Rating_Process"),
                Rack_No_Storage_Location = Val<string>("Rack_No_Storage_Location", ""),
                Issue_with_Material = Val<string>("Issue_with_Material", ""),
                Date_of_Supp_Return = Val<string>("Date_of_Supp_Return", ""),
                CAPA_Action_Plan_recd_from_Supplier =
                    Val<string>("CAPA_Action_Plan_recd_from_Supplier", ""),
                Action_Plan_Apporved_by_AE_Quality_Dept =
                    Val<string>("Action_Plan_Apporved_by_AE_Quality_Dept", ""),
                Action_Plan_for_Supplier =
                    Val<string>("Action_Plan_for_Supplier", ""),
                Entry_Date = Val<string>("Entry_Date", ""),
                CreatedBy = Val<string>("CreatedBy", "")
            };
        }

        private SupplierPOStatusModel MapReporting(SqlDataReader dr)
        {
            var m = Map(dr);
            try { m.Days_Material = Convert.ToInt32(dr["Days_Material"]); }
            catch { m.Days_Material = 0; }
            return m;
        }
    }
}
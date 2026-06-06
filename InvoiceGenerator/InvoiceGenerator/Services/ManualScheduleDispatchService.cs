// Services/ManualScheduleDispatchService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ManualScheduleDispatchService
    {
        private readonly string _conn;

        public ManualScheduleDispatchService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (data entry list) ──
        public List<ManualScheduleDispatchModel> GetAll(
            int deptId, int monthId)
        {
            var list = new List<ManualScheduleDispatchModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MANUAL_CUSTOMER_SCHEDULE_VS_DISPATCH_DATA_ENTRY",
                con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(MapEntry(dr));
            return list;
        }

        // ── SELECT REPORT (day-wise pivot) ──
        public List<ManualScheduleReportModel> GetReport(
            int deptId, int customerId, int monthId, int yearId)
        {
            var list = new List<ManualScheduleReportModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MANUAL_CUSTOMER_SCHEDULE_VS_DISPATCH_DATA_ENTRY",
                con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Report");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            cmd.Parameters.AddWithValue("@YearId", yearId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(MapReport(dr));
            return list;
        }

        //// ── GET ALL CUSTOMERS ──
        //public List<object> GetCustomers()
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT CustomerId, Customer " +
        //        "FROM CustomerMaster ORDER BY Customer", con);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            customerId = Convert.ToInt32(dr["CustomerId"]),
        //            customer = dr["Customer"].ToString()
        //        });
        //    return list;
        //}
        public List<object> GetCustomers()
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Customer");
            cmd.Parameters.AddWithValue("@DepartmentId", 0);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

            con.Open();

            using var dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                list.Add(new
                {
                    customerId = Convert.ToInt32(dr["CustomerId"]),
                    customer = dr["Customer"].ToString()
                });
            }

            return list;
        }
        // ── GET ITEMS BY DEPT + CUSTOMER (via Matrix) ──
        //public List<object> GetItemsByDeptCustomer(
        //    int deptId, int customerId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT i.ItemId, i.Item_Code, i.Item_Description " +
        //        "FROM Item_Description_Master i " +
        //        "INNER JOIN Matrix_Item_Customer_Master m " +
        //        "   ON m.ItemId = i.ItemId " +
        //        "WHERE m.DepartmentId = @d AND m.CustomerId = @c " +
        //        "ORDER BY i.Item_Code", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
        //    cmd.Parameters.AddWithValue("@c", customerId);
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
        public List<object> GetItemsByDeptCustomer(int deptId, int customerId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDeptCust");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
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
            ManualScheduleDispatchModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Record saved successfully.");
        }

        // ── UPDATE ──
        public void Update(
            ManualScheduleDispatchModel m, string updatedBy)
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
                "SP_MANUAL_CUSTOMER_SCHEDULE_VS_DISPATCH_DATA_ENTRY",
                con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            ManualScheduleDispatchModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_MANUAL_CUSTOMER_SCHEDULE_VS_DISPATCH_DATA_ENTRY",
                con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@CustomerId", m.CustomerId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Tentative_PO_Qty", m.Tentative_PO_Qty);
            cmd.Parameters.AddWithValue("@Req_Quantity", m.Req_Quantity);
            cmd.Parameters.AddWithValue("@Dis_Quantity", m.Dis_Quantity);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@MonthId",
                string.IsNullOrEmpty(m.Date)
                    ? DateTime.Now.Month
                    : DateTime.Parse(m.Date).Month);
            cmd.Parameters.AddWithValue("@YearId",
                string.IsNullOrEmpty(m.Date)
                    ? DateTime.Now.Year
                    : DateTime.Parse(m.Date).Year);
            return cmd;
        }

        // ── MAP ENTRY ──
        private ManualScheduleDispatchModel MapEntry(SqlDataReader dr)
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
            return new ManualScheduleDispatchModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                CustomerId = Val<int>("CustomerId"),
                ItemId = Val<int>("ItemId"),
                Tentative_PO_Qty = Val<int>("Tentative_PO_Qty"),
                Req_Quantity = Val<int>("Req_Quantity"),
                Dis_Quantity = Val<int>("Dis_Quantity"),
                IsActive = Val<int>("IsActive"),
                CreatedBy = Val<string>("CreatedBy", ""),
                Department = Val<string>("Department", ""),
                Customer = Val<string>("Customer", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Date = Val<string>("Date", "")
            };
        }

        // ── MAP REPORT ──
        private ManualScheduleReportModel MapReport(SqlDataReader dr)
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

            var m = new ManualScheduleReportModel
            {
                DepartmentId = Val<int>("DepartmentId"),
                Department = Val<string>("Department", ""),
                CustomerId = Val<int>("CustomerId"),
                Customer = Val<string>("Customer", ""),
                ItemId = Val<int>("ItemId"),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                MIN_Stock = Val<int?>("MIN_Stock"),
                MAX_Stock = Val<int?>("MAX_Stock"),
                Month_Total_Req_Qty = Val<int>("Month_Total_Req_Qty"),
                Month_Total_Dis_Qty = Val<int>("Month_Total_Dis_Qty"),
                Tentative_PO_Qty = Val<int>("Tentative_PO_Qty")
            };

            // Read Day1..Day31 Req + Dis
            for (int d = 1; d <= 31; d++)
            {
                m.DayReq[d] = Val<int>($"Day{d}_Req");
                m.DayDis[d] = Val<int>($"Day{d}_Dis");
            }

            return m;
        }
    }
}
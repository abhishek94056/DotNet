// Services/CustomerDispatchService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class CustomerDispatchService
    {
        private readonly string _conn;

        public CustomerDispatchService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (by dept + month) ──
        public List<CustomerDispatchModel> GetAll(int deptId, int monthId)
        {
            var list = new List<CustomerDispatchModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_CUSTOMER_DISPATCH_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── GET PRODUCE QTY ──
        public int GetProduceQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_CUSTOMER_DISPATCH_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ProduceQty");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
        }

        // ── GET DISPATCH QTY (already dispatched this month) ──
        public int GetDispatchQty(int deptId, int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_CUSTOMER_DISPATCH_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_DispatchQty");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0 : Convert.ToInt32(result);
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
        //// ── GET ITEMS BY DEPT + CUSTOMER (from Matrix) ──
        //public List<object> GetItemsByDeptCustomer(int deptId, int customerId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    // Items linked to this customer via Matrix_Item_Customer_Master
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
        // ── GET ITEM DESCRIPTION ──
        //public object? GetItemDescription(int itemId)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT Item_Description FROM Item_Description_Master " +
        //        "WHERE ItemId = @i", con);
        //    cmd.Parameters.AddWithValue("@i", itemId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    if (!dr.Read()) return null;
        //    return new
        //    {
        //        item_Description = dr["Item_Description"].ToString()
        //    };
        //}
        public object? GetItemDescription(int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Action", "GetItemDescription");
            cmd.Parameters.AddWithValue("@DepartmentId", 0);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", itemId);

            con.Open();

            using var dr = cmd.ExecuteReader();

            if (!dr.Read())
                return null;

            return new
            {
                item_Description = dr["Item_Description"].ToString()
            };
        }
        // ── INSERT ──
        public (bool success, string message) Insert(
            CustomerDispatchModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Dispatch saved successfully.");
        }

        // ── UPDATE ──
        public void Update(CustomerDispatchModel m, string updatedBy)
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
                "SP_CUSTOMER_DISPATCH_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            CustomerDispatchModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_CUSTOMER_DISPATCH_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@CustomerId", m.CustomerId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@GRN_Status", m.GRN_Status ?? "NO");
            cmd.Parameters.AddWithValue("@MonthId",
                string.IsNullOrEmpty(m.Date)
                    ? DateTime.Now.Month
                    : DateTime.Parse(m.Date).Month);
            return cmd;
        }

        private CustomerDispatchModel Map(SqlDataReader dr)
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
            return new CustomerDispatchModel
            {
                SrNo = Val<int>("SrNo"),
                CustomerId = Val<int>("CustomerId"),
                ItemId = Val<int>("ItemId"),
                Quantity = Val<int>("Quantity"),
                DepartmentId = Val<int>("DepartmentId"),
                GRN_Status = Val<string>("GRN_Status", "NO"),
                CreatedBy = Val<string>("CreatedBy", ""),
                Customer = Val<string>("Customer", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Department = Val<string>("Department", ""),
                ShiftName = Val<string>("ShiftName", ""),
                Add_Date = Val<string>("Add_Date", ""),
                Invocie_No = Val<string>("Invocie_No", ""),
                IsActive = Val<int>("IsActive")
            };
        }
    }
}
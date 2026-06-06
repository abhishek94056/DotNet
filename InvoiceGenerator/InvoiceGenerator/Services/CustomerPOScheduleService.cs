// Services/CustomerPOScheduleService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class CustomerPOScheduleService
    {
        private readonly string _conn;

        public CustomerPOScheduleService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (current month) ──
        public List<CustomerPOScheduleModel> GetAll(int departmentId)
        {
            var list = new List<CustomerPOScheduleModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_CUSTOMER_PO_SCHEDULE_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(MapSchedule(dr));
            return list;
        }
        // ── GET ITEM RATE BY ITEM ID ──
        public decimal GetItemRate(int itemId)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_CUSTOMER_PO_SCHEDULE_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectBy_ItemId");
            cmd.Parameters.AddWithValue("@ItemId", itemId);
            con.Open();
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value
                ? 0m : Convert.ToDecimal(result);
        }
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
        public List<object> GetCustomers()
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con);

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
        // ── INSERT ──
        public (bool success, string message) Insert(
            CustomerPOScheduleModel m, string createdBy)
        {
            if (m.Quantity == 0)
                return (false, "Quantity cannot be zero.");

            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "PO Schedule saved successfully.");
        }

        // ── UPDATE ──
        public void Update(CustomerPOScheduleModel m, string updatedBy)
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
                "SP_CUSTOMER_PO_SCHEDULE_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            CustomerPOScheduleModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_CUSTOMER_PO_SCHEDULE_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@CustomerId", m.CustomerId);
            cmd.Parameters.AddWithValue("@PO_Number", m.PO_Number ?? "");
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@PO_Item_Rate", m.PO_Item_Rate);
            cmd.Parameters.AddWithValue("@PO_Date",
                string.IsNullOrEmpty(m.PO_Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.PO_Date));
            cmd.Parameters.AddWithValue("@Delivery_Date",
                string.IsNullOrEmpty(m.Delivery_Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Delivery_Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            return cmd;
        }

        private CustomerPOScheduleModel MapSchedule(SqlDataReader dr)
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
            return new CustomerPOScheduleModel
            {
                SrNo = Val<int>("SrNo"),
                CustomerId = Val<int>("CustomerId"),
                Customer = Val<string>("Customer", ""),
                PO_Number = Val<string>("PO_Number", ""),
                ItemId = Val<int>("ItemId"),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Quantity = Val<int>("Quantity"),
                PO_Item_Rate = Val<decimal>("PO_Item_Rate"),
                PO_Date = Val<string>("PO_Date", ""),
                Delivery_Date = Val<string>("Delivery_Date", ""),
                Date = Val<string>("Date", ""),
                Plan_Date = Val<string>("Plan_Date", ""),
                DepartmentId = Val<int>("DepartmentId"),
                DepartmentName = Val<string>("Department", "")
            };
        }
    }
}
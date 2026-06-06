// Services/MatrixItemCustomerService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class MatrixItemCustomerService
    {
        private readonly string _conn;

        public MatrixItemCustomerService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<MatrixItemCustomerModel> GetAll(int departmentId)
        {
            var list = new List<MatrixItemCustomerModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MATRIX_ITEM_CUSTOMER_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── GET BY ID ──
        public MatrixItemCustomerFormModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MATRIX_ITEM_CUSTOMER_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectOne");
            cmd.Parameters.AddWithValue("@MatrixId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new MatrixItemCustomerFormModel
            {
                MatrixId = Convert.ToInt32(dr["MatrixId"]),
                CustomerId = dr["CustomerId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CustomerId"]),
                ItemId = dr["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemId"]),
                DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
                Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
                CreatedBy = dr["CreatedBy"].ToString()!
            };
        }

        // ── GET ALL CUSTOMERS ──
        public List<object> GetAllCustomers()
        {
            var list = new List<object>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Customer");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new
                {
                    customerId = Convert.ToInt32(dr["CustomerId"]),
                    customer = dr["Customer"].ToString()
                });
            return list;
        }

        // ── GET ITEMS BY DEPARTMENT ──
        public List<object> GetItemsByDepartment(int deptId)
        {
            var list = new List<object>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new
                {
                    itemId = Convert.ToInt32(dr["ItemId"]),
                    item_Code = dr["Item_Code"].ToString(),
                    item_Description = dr["Item_Description"].ToString()
                });
            return list;
        }

        // ── INSERT ──
        public (bool success, string message, int id) Insert(
            MatrixItemCustomerFormModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MATRIX_ITEM_CUSTOMER_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@CustomerId", m.CustomerId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            //cmd.Parameters.AddWithValue("@Date", m.Date);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            con.Open();
           
            int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

            if (rows > 0)
                return (true, "Matrix saved successfully.", 0);

            // ⚠️ Could be duplicate OR failure
            return (false, "Matrix already exists.", 0);
        }

        // ── UPDATE ──
        public void Update(MatrixItemCustomerFormModel m, string updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MATRIX_ITEM_CUSTOMER_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@MatrixId", m.MatrixId);
            cmd.Parameters.AddWithValue("@CustomerId", m.CustomerId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            //cmd.Parameters.AddWithValue("@Date", m.Date);
            cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── DELETE ──
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_MATRIX_ITEM_CUSTOMER_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@MatrixId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private MatrixItemCustomerModel Map(SqlDataReader dr) => new()
        {
            MatrixId = Convert.ToInt32(dr["MatrixId"]),
            CustomerId = dr["CustomerId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["CustomerId"]),
            Customer = dr["Customer"].ToString()!,
            ItemId = dr["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,
            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
            DepartmentName = dr["Department"].ToString()!,
            Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
            CreatedBy = dr["CreatedBy"].ToString()!
        };
    }
}
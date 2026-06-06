// Services/CustomerService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class CustomerService
    {
        private readonly string _conn;

        public CustomerService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<CustomerModel> GetAll()
        {
            var list = new List<CustomerModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(Map(dr));
            return list;
        }

        // ── GET BY ID ──
        public CustomerModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");  //SelectOne
            cmd.Parameters.AddWithValue("@CustomerId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Map(dr) : null;
        }
        //    public (bool success, string message, int id) Insert(
        //CustomerModel c, string createdBy)
        //    {
        //        using var con = new SqlConnection(_conn);
        //        using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
        //        { CommandType = CommandType.StoredProcedure };

        //        cmd.Parameters.AddWithValue("@Action", "Insert");
        //        cmd.Parameters.AddWithValue("@Customer", (object?)c.Customer?.Trim() ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@Address", (object?)c.Address ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@StateCode", (object?)c.StateCode ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@State", (object?)c.State ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@GSTIN", (object?)c.GSTIN ?? DBNull.Value);
        //        cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

        //        con.Open();

        //        int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

        //        if (rows > 0)
        //            return (true, "Customer saved successfully.", 0);

        //        // ⚠️ Could be duplicate OR failure
        //        return (false, "Customer already exists.", 0);
        //    }

        public (bool success, string message, int id) Insert(CustomerModel c, string createdBy)
        {
            var exists = GetAll()
                .Any(x => x.Customer.Trim().ToLower() == c.Customer.Trim().ToLower());

            if (exists)
                return (false, "Customer already exists.", 0);

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@Customer", (object?)c.Customer?.Trim() ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Address", (object?)c.Address ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@StateCode", (object?)c.StateCode ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@State", (object?)c.State ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@GSTIN", (object?)c.GSTIN ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Customer saved successfully.", 0);
        }
        // ── UPDATE ──
        //public void Update(CustomerModel c, string updatedBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "Update");
        //    cmd.Parameters.AddWithValue("@CustomerId", c.CustomerId);
        //    cmd.Parameters.AddWithValue("@Customer", c.Customer.Trim());
        //    cmd.Parameters.AddWithValue("@Address", c.Address);
        //    cmd.Parameters.AddWithValue("@StateCode", c.StateCode);
        //    cmd.Parameters.AddWithValue("@State", c.State);
        //    cmd.Parameters.AddWithValue("@GSTIN", c.GSTIN);
        //    cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);
        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //}

        public (bool success, string message) Update(CustomerModel c, string updatedBy)
        {
            var exists = GetAll()
                .Any(x => x.Customer.Trim().ToLower() == c.Customer.Trim().ToLower()
                       && x.CustomerId != c.CustomerId); // ✅ ignore same record

            if (exists)
                return (false, "Customer already exists.");

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@CustomerId", c.CustomerId);
            cmd.Parameters.AddWithValue("@Customer", c.Customer.Trim());
            cmd.Parameters.AddWithValue("@Address", c.Address);
            cmd.Parameters.AddWithValue("@StateCode", c.StateCode);
            cmd.Parameters.AddWithValue("@State", c.State);
            cmd.Parameters.AddWithValue("@GSTIN", c.GSTIN);
            cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Customer updated successfully.");
        }

        // ── DELETE ──
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_CUSTOMER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@CustomerId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private CustomerModel Map(SqlDataReader dr) => new()
        {
            CustomerId = Convert.ToInt32(dr["CustomerId"]),
            Customer = dr["Customer"].ToString(),
            Address = dr["Address"].ToString(),
            StateCode = dr["StateCode"] == DBNull.Value ? null : Convert.ToInt32(dr["StateCode"]),
            State = dr["State"].ToString(),
            GSTIN = dr["GSTIN"].ToString(),
            CreatedBy = dr["CreatedBy"].ToString()
        };
    }
}
// Services/SupplierService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class SupplierService
    {
        private readonly string _conn;

        public SupplierService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<SupplierModel> GetAll()
        {
            var list = new List<SupplierModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(Map(dr));
            return list;
        }

        // ── GET BY ID ──
        public SupplierModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectOne");
            cmd.Parameters.AddWithValue("@SupplierId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Map(dr) : null;
        }

        // ── INSERT ──
        //public (bool success, string message, int id) Insert(
        //    SupplierModel s, string createdBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
        //    { CommandType = CommandType.StoredProcedure };

        //    cmd.Parameters.AddWithValue("@Action", "Insert");
        //    cmd.Parameters.AddWithValue("@Supplier", s.Supplier.Trim());
        //    cmd.Parameters.AddWithValue("@Address", s.Address);
        //    cmd.Parameters.AddWithValue("@StateCode", s.StateCode);
        //    cmd.Parameters.AddWithValue("@State", s.State);
        //    cmd.Parameters.AddWithValue("@GSTIN", s.GSTIN);
        //    //cmd.Parameters.AddWithValue("@Date", s.Date);
        //    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
        //    con.Open();
        //    //int newId = Convert.ToInt32(cmd.ExecuteScalar());
        //    //int rows = cmd.ExecuteNonQuery();
        //    //if (rows == 0)
        //    //    return (false, "Supplier name already exists.", 0);
        //    //return (true, "Supplier saved successfully.", 0);
        //    int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

        //    if (rows > 0)
        //        return (true, "Supplier saved successfully.", 0);

        //    // ⚠️ Could be duplicate OR failure
        //    return (false, "Supplier already exists.", 0);
        //}

        public (bool success, string message, int id) Insert(SupplierModel s, string createdBy)
        {
            var exists = GetAll()
                .Any(x => x.Supplier.Trim().ToLower() == s.Supplier.Trim().ToLower());

            if (exists)
                return (false, "Supplier already exists.", 0);

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@Supplier", s.Supplier.Trim());
            cmd.Parameters.AddWithValue("@Address", s.Address);
            cmd.Parameters.AddWithValue("@StateCode", s.StateCode);
            cmd.Parameters.AddWithValue("@State", s.State);
            cmd.Parameters.AddWithValue("@GSTIN", s.GSTIN);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Supplier saved successfully.", 0);
        }

        // ── UPDATE ──
        //public void Update(SupplierModel s, string updatedBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "Update");
        //    cmd.Parameters.AddWithValue("@SupplierId", s.SupplierId);
        //    cmd.Parameters.AddWithValue("@Supplier", s.Supplier.Trim());
        //    cmd.Parameters.AddWithValue("@Address", s.Address);
        //    cmd.Parameters.AddWithValue("@StateCode", s.StateCode);
        //    cmd.Parameters.AddWithValue("@State", s.State);
        //    cmd.Parameters.AddWithValue("@GSTIN", s.GSTIN);
        //    //cmd.Parameters.AddWithValue("@Date", s.Date);
        //    cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);
        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //}

        public (bool success, string message) Update(SupplierModel s, string updatedBy)
        {
            var exists = GetAll()
                .Any(x => x.Supplier.Trim().ToLower() == s.Supplier.Trim().ToLower()
                       && x.SupplierId != s.SupplierId); // ✅ ignore current row

            if (exists)
                return (false, "Supplier already exists.");

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@SupplierId", s.SupplierId);
            cmd.Parameters.AddWithValue("@Supplier", s.Supplier.Trim());
            cmd.Parameters.AddWithValue("@Address", s.Address);
            cmd.Parameters.AddWithValue("@StateCode", s.StateCode);
            cmd.Parameters.AddWithValue("@State", s.State);
            cmd.Parameters.AddWithValue("@GSTIN", s.GSTIN);
            cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Supplier updated successfully.");
        }

        // ── DELETE ──
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_SUPPLIER_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SupplierId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private SupplierModel Map(SqlDataReader dr) => new()
        {
            SupplierId = Convert.ToInt32(dr["SupplierId"]),
            Supplier = dr["Supplier"].ToString(),
            Address = dr["Address"].ToString(),
            StateCode = Convert.ToInt32(dr["StateCode"]),
            State = dr["State"].ToString(),
            GSTIN = dr["GSTIN"].ToString(),
            //Date = Convert.ToDateTime(dr["Date"]),
            CreatedBy = dr["CreatedBy"].ToString()
        };
    }
}
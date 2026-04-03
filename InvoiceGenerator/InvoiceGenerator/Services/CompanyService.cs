using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    // Services/CompanyService.cs
    public class CompanyService : ICompanyService
    {
        private readonly string _conn;
        public CompanyService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator");

        // GET ALL
        public List<CompanyModel> GetAll()
        {
            var list = new List<CompanyModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetCompanies", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(Map(dr));
            return list;
        }

        // GET BY ID
        public CompanyModel GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetCompanyById", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CompanyId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Map(dr) : null;
        }

        // INSERT
        public int Insert(CompanyModel c)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_InsertCompany", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CompanyName", c.CompanyName);
            cmd.Parameters.AddWithValue("@Address", c.Address);
            cmd.Parameters.AddWithValue("@StateCode", c.StateCode);
            cmd.Parameters.AddWithValue("@State", c.State);
            cmd.Parameters.AddWithValue("@GSTIN", c.GSTIN);
            cmd.Parameters.AddWithValue("@PaymentTerm", c.PaymentTerm);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // UPDATE
        public void Update(CompanyModel c)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_UpdateCompany", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CompanyId", c.CompanyId);
            cmd.Parameters.AddWithValue("@CompanyName", c.CompanyName);
            cmd.Parameters.AddWithValue("@Address", c.Address);
            cmd.Parameters.AddWithValue("@StateCode", c.StateCode);
            cmd.Parameters.AddWithValue("@State", c.State);
            cmd.Parameters.AddWithValue("@GSTIN", c.GSTIN);
            cmd.Parameters.AddWithValue("@PaymentTerm", c.PaymentTerm);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_DeleteCompany", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@CompanyId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private CompanyModel Map(SqlDataReader dr) => new()
        {
            CompanyId = Convert.ToInt32(dr["Id"]),
            CompanyName = dr["CompanyName"].ToString(),
            Address = dr["Address"].ToString(),
            StateCode = Convert.ToInt32(dr["StateCode"]),
            State = dr["State"].ToString(),
            GSTIN = dr["GSTIN"].ToString(),
            PaymentTerm = dr["PaymentTerm"].ToString()
        };
    }
}

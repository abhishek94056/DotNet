// Services/TransportService.cs
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class TransportService : ITransportService
    {
        private readonly string _conn;

        public TransportService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator");

        // GET ALL
        public List<TransportModel> GetAll()
        {
            var list = new List<TransportModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetTransportModes", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(Map(dr));
            return list;
        }

        // GET BY ID
        public TransportModel GetById(int Id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetTransportModeById", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", Id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Map(dr) : null;
        }

        // INSERT
        public int Insert(TransportModel t)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_InsertTransportMode", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ModeName", t.ModeName);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // UPDATE
        public void Update(TransportModel t)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_UpdateTransportMode", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", t.Id);
            cmd.Parameters.AddWithValue("@ModeName", t.ModeName);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void Delete(int Id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_DeleteTransportMode", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Id", Id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private TransportModel Map(SqlDataReader dr) => new()
        {
            Id = Convert.ToInt32(dr["Id"]),
            ModeName = dr["ModeName"].ToString()
        };
    }
}
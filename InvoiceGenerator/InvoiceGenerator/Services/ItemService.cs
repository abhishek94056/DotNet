// Services/ItemService.cs
using InvoiceGenerator.Interfaces;
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ItemService : IItemService
    {
        private readonly string _conn;

        public ItemService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator");

        // GET ALL
        public List<ItemModel> GetAll()
        {
            var list = new List<ItemModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetItems", con)
            { CommandType = CommandType.StoredProcedure };
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(Map(dr));
            return list;
        }

        // GET BY ID
        public ItemModel GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_GetItemById", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Map(dr) : null;
        }

        // INSERT
        public int Insert(ItemModel item)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_InsertInvoiceItem", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode);
            cmd.Parameters.AddWithValue("@ItemDescription", item.ItemDescription);
            cmd.Parameters.AddWithValue("@HSNCode", item.HSNCode);
            cmd.Parameters.AddWithValue("@Rate", item.Rate);
            cmd.Parameters.AddWithValue("@GST", item.GST);
            con.Open();
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        // UPDATE
        public void Update(ItemModel item)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_UpdateInvoiceItem", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ItemId", item.ItemId);
            cmd.Parameters.AddWithValue("@ItemCode", item.ItemCode);
            cmd.Parameters.AddWithValue("@ItemDescription", item.ItemDescription);
            cmd.Parameters.AddWithValue("@HSNCode", item.HSNCode);
            cmd.Parameters.AddWithValue("@Rate", item.Rate);
            cmd.Parameters.AddWithValue("@GST", item.GST);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // DELETE
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("sp_DeleteInvoiceItem", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private ItemModel Map(SqlDataReader dr) => new()
        {
            ItemId = Convert.ToInt32(dr["ItemId"]),
            ItemCode = dr["ItemCode"].ToString(),
            ItemDescription = dr["ItemDescription"].ToString(),
            HSNCode = dr["HSNCode"].ToString(),
            Rate = Convert.ToDecimal(dr["Rate"]),
            GST = Convert.ToDecimal(dr["GST"])
        };
    }
}
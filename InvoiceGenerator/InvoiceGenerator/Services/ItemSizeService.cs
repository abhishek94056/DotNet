// Services/ItemSizeService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ItemSizeService
    {
        private readonly string _conn;

        public ItemSizeService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<ItemSizeModel> GetAll()
        {
            var list = new List<ItemSizeModel>();
            using var con = new SqlConnection(_conn);
            int deptId = 0;

            using (var cmdDept = new SqlCommand(
                @"SELECT TOP 1 DepartmentId FROM Item_Size_Master WHERE IsActive = 0", con))
            {
                con.Open();
                var obj = cmdDept.ExecuteScalar();
                deptId = obj != null ? Convert.ToInt32(obj) : 0;
                con.Close();
            }
            using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── GET BY ID ──
        public ItemSizeModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Size_By_SizeCode");
            cmd.Parameters.AddWithValue("@SizeId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? MapRaw(dr) : null;
        }

        //public List<DepartmentModel> GetDepartments()
        //{
        //    var list = new List<DepartmentModel>();

        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT DepartmentId, Department FROM Department_Master",
        //        con);

        //    con.Open();

        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        list.Add(new DepartmentModel
        //        {
        //            DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
        //            Department = dr["Department"]?.ToString() ?? ""
        //        });
        //    }

        //    return list;
        //}
        public List<DepartmentModel> GetDepartments()
        {
            var list = new List<DepartmentModel>();

            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new DepartmentModel
                {
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    Department = dr["Department"]?.ToString() ?? ""
                });
            }

            return list;
        }
        //   public (bool success, string message, int id) Insert(
        //ItemSizeModel m, string createdBy)
        //   {
        //       using var con = new SqlConnection(_conn);
        //       using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
        //       {
        //           CommandType = CommandType.StoredProcedure
        //       };

        //       cmd.Parameters.Add("@Action", SqlDbType.VarChar).Value = "Insert";
        //       cmd.Parameters.Add("@DepartmentId", SqlDbType.Int).Value = m.DepartmentId;
        //       cmd.Parameters.Add("@ItemSize_Code", SqlDbType.VarChar).Value = (m.ItemSize_Code ?? "").Trim();
        //       cmd.Parameters.Add("@Item_Size", SqlDbType.VarChar).Value = (m.Item_Size ?? "").Trim();
        //       cmd.Parameters.Add("@Rate", SqlDbType.Decimal).Value = m.Rate;
        //       cmd.Parameters.Add("@CreatedBy", SqlDbType.VarChar).Value = createdBy;
        //       cmd.Parameters.Add("@MIN_Stock", SqlDbType.Decimal).Value = m.MIN_Stock;
        //       cmd.Parameters.Add("@MAX_Stock", SqlDbType.Decimal).Value = m.MAX_Stock;

        //       con.Open();

        //       //int result = Convert.ToInt32(cmd.ExecuteScalar());

        //       //if (result == -1)
        //       int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

        //       if (rows > 0)
        //           return (true, "Item Size saved successfully.", 0);

        //       // ⚠️ Could be duplicate OR failure
        //       return (false, "Item Size already exists", 0);
        //   }

        public (bool success, string message, int id) Insert(ItemSizeModel m, string createdBy)
        {
            var exists = GetAll()
                .Any(x => x.ItemSize_Code.Trim().ToUpper() == m.ItemSize_Code.Trim().ToUpper());

            if (exists)
                return (false, "Item Size Code already exists.", 0);

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@ItemSize_Code", m.ItemSize_Code.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@Item_Size", m.Item_Size.Trim());
            cmd.Parameters.AddWithValue("@Rate", m.Rate);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
            cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Item Size saved successfully.", 0);
        }
        // ── UPDATE ──
        //public void Update(ItemSizeModel m, string updatedBy)
        //{
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "Update");
        //    cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
        //    cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
        //    cmd.Parameters.AddWithValue("@ItemSize_Code", (m.ItemSize_Code ?? "").Trim());
        //    cmd.Parameters.AddWithValue("@Item_Size", (m.Item_Size ?? "").Trim());
        //    cmd.Parameters.AddWithValue("@Rate", m.Rate);
        //    cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);
        //    cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
        //    cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);

        //    con.Open();
        //    cmd.ExecuteNonQuery();
        //}

        public (bool success, string message) Update(ItemSizeModel m, string updatedBy)
        {
            var exists = GetAll()
                .Any(x => x.ItemSize_Code.Trim().ToUpper() == m.ItemSize_Code.Trim().ToUpper()
                       && x.SizeId != m.SizeId); // ✅ ignore current record

            if (exists)
                return (false, "Item Size Code already exists.");

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@ItemSize_Code", m.ItemSize_Code.Trim().ToUpper());
            cmd.Parameters.AddWithValue("@Item_Size", m.Item_Size.Trim());
            cmd.Parameters.AddWithValue("@Rate", m.Rate);
            cmd.Parameters.AddWithValue("@CreatedBy", updatedBy);
            cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
            cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);

            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Item Size updated successfully.");
        }

        // ── DELETE ──
        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_SIZE_MASTER", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SizeId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private ItemSizeModel Map(SqlDataReader dr) => new()
        {
            SizeId = Convert.ToInt32(dr["SizeId"]),
            DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
            Department = dr["Department"]?.ToString() ?? "",

            ItemSize_Code = dr["ItemSize_Code"]?.ToString() ?? "",
            Item_Size = dr["Item_Size"]?.ToString() ?? "",

            Rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Rate"]),
            CreatedBy = dr["CreatedBy"]?.ToString() ?? "",

            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MAX_Stock"]),

            IsActive = dr["IsActive"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IsActive"])
        };

        private ItemSizeModel MapRaw(SqlDataReader dr) => new()
        {
            SizeId = Convert.ToInt32(dr["SizeId"]),
            DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
            

            ItemSize_Code = dr["ItemSize_Code"]?.ToString() ?? "",
            Item_Size = dr["Item_Size"]?.ToString() ?? "",

            Rate = dr["Rate"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Rate"]),
            CreatedBy = dr["CreatedBy"]?.ToString() ?? "",

            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MAX_Stock"]),

            IsActive = dr["IsActive"] == DBNull.Value ? 0 : Convert.ToInt32(dr["IsActive"])
        };
    }
}
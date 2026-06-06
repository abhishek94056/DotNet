// Services/MatrixItemSizeService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class MatrixItemSizeService
    {
        private readonly string _conn;

        public MatrixItemSizeService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<MatrixItemSizeModel> GetAll(int departmentId)
        {
            var list = new List<MatrixItemSizeModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_MATRIX_ITEM_SIZE_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── GET BY ID ──
        public MatrixItemSizeFormModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_MATRIX_ITEM_SIZE_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectOne");
            cmd.Parameters.AddWithValue("@MatrixId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            if (!dr.Read()) return null;
            return new MatrixItemSizeFormModel
            {
                MatrixId = Convert.ToInt32(dr["MatrixId"]),
                SizeId = dr["SizeId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SizeId"]),
                ItemId = dr["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemId"]),
                DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
                Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
                CreatedBy = dr["CreatedBy"].ToString()!
            };
        }

        // ── GET ITEMS BY DEPARTMENT ──
        public List<object> GetItemsByDepartment(int deptId)
        {
            var list = new List<object>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
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

        // ── GET SIZES BY DEPARTMENT ──
        public List<object> GetSizesByDepartment(int deptId)
        {
            var list = new List<object>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemSizeByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new
                {
                    sizeId = Convert.ToInt32(dr["SizeId"]),
                    itemSize_Code = dr["ItemSize_Code"].ToString(),
                    item_Size = dr["Item_Size"].ToString()
                });
            return list;
        }

        // ── INSERT ──
        public (bool success, string message, int id) Insert(
            MatrixItemSizeFormModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_MATRIX_ITEM_SIZE_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Insert");
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            //cmd.Parameters.AddWithValue("@Date", m.Date);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            con.Open();
         
            int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

            if (rows > 0)
                return (true, "Item saved successfully.", 0);

            // ⚠️ Could be duplicate OR failure
            return (false, "Item already exists.", 0);
        }

        // ── UPDATE ──
        public void Update(MatrixItemSizeFormModel m, string updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_MATRIX_ITEM_SIZE_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@MatrixId", m.MatrixId);
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
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
            using var cmd = new SqlCommand("SP_MATRIX_ITEM_SIZE_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@MatrixId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private MatrixItemSizeModel Map(SqlDataReader dr) => new()
        {
            MatrixId = Convert.ToInt32(dr["MatrixId"]),
            SizeId = dr["SizeId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SizeId"]),
            ItemSize_Code = dr["ItemSize_Code"].ToString()!,
            Item_Size = dr["Item_Size"].ToString()!,
            ItemId = dr["ItemId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,
            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
            Department = dr["Department"].ToString()!,
            //Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
            CreatedBy = dr["CreatedBy"].ToString()!
        };
    }
}
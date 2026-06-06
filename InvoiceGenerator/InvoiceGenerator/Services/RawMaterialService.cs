// Services/RawMaterialService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class RawMaterialService
    {
        private readonly string _conn;

        public RawMaterialService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── GET ALL ──
        public List<RawMaterialModel> GetAll(int departmentId, int monthId)
        {
            var list = new List<RawMaterialModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_RAW_MATERIAL_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            cmd.Parameters.AddWithValue("@MonthId", monthId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        public List<object> GetSizesByDept(int deptId)
        {
            var list = new List<object>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemSizeByDept");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            cmd.Parameters.AddWithValue("@CustomerId", 0);
            cmd.Parameters.AddWithValue("@ItemId", 0);

            con.Open();

            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new
                {
                    sizeId = Convert.ToInt32(dr["SizeId"]),
                    itemSize_Code = dr["ItemSize_Code"].ToString(),
                    item_Size = dr["Item_Size"].ToString(),
                    rate = dr["Rate"] == DBNull.Value
                        ? 0m
                        : Convert.ToDecimal(dr["Rate"])
                });
            }

            return list;
        }
        // ── INSERT ──
        public (bool success, string message, int id) Insert(
            RawMaterialModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();

            return (true, "Raw Material added successfully.", 1);
        }

        // ── UPDATE ──
        public void Update(RawMaterialModel m, string updatedBy)
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
                "SP_RAW_MATERIAL_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            RawMaterialModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_RAW_MATERIAL_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@RM_Rate", m.RM_Rate);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            return cmd;
        }

        // ── MAP ──
        private RawMaterialModel Map(SqlDataReader dr)
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

            return new RawMaterialModel
            {
                SrNo = Val<int>("SrNo"),
                SizeId = Val<int>("SizeId"),
                Quantity = Val<decimal>("Quantity"),
                RM_Rate = Val<decimal>("RM_Rate"),
                DepartmentId = Val<int>("DepartmentId"),
                CreatedBy = Val<string>("CreatedBy", ""),
                ItemSize_Code = Val<string>("ItemSize_Code", ""),
                Item_Size = Val<string>("Item_Size", ""),
                Add_Date = Val<string>("Add_Date", ""),
                DepartmentName = Val<string>("Department", ""),
                Total_Value = Val<decimal>("Total_Value"),
                // ── View-only ──
                SupplierId = Val<int>("SupplierId"),
                Supplier = Val<string>("Supplier", ""),
                PO_Number = Val<string>("PO_Number", ""),
                Invoice_No = Val<string>("Invoice_No", ""),
                Batch_Number = Val<string>("Batch_Number", ""),
                IsActive = Val<int>("IsActive")
            };
        }
    }
}
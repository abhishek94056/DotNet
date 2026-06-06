// Services/ProductionFGIssueService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ProductionFGIssueService
    {
        private readonly string _conn;

        public ProductionFGIssueService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (current month by dept) ──
        public List<ProductionFGIssueModel> GetAll(int deptId)
        {
            var list = new List<ProductionFGIssueModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SP_PRODUCTION_FGISSUE_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        //// ── GET ITEMS BY DEPT ──
        //public List<object> GetItemsByDept(int deptId)
        //{
        //    var list = new List<object>();
        //    using var con = new SqlConnection(_conn);
        //    using var cmd = new SqlCommand(
        //        "SELECT ItemId, Item_Code, Item_Description " +
        //        "FROM Item_Description_Master " +
        //        "WHERE DepartmentId = @d ORDER BY Item_Code", con);
        //    cmd.Parameters.AddWithValue("@d", deptId);
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new
        //        {
        //            itemId = Convert.ToInt32(dr["ItemId"]),
        //            item_Code = dr["Item_Code"].ToString(),
        //            item_Description = dr["Item_Description"].ToString()
        //        });
        //    return list;
        //}

        public List<object> GetItemsByDept(int deptId)
        {
            var list = new List<object>();

            using (SqlConnection con = new SqlConnection(_conn))
            {
                using (SqlCommand cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDept");
                    cmd.Parameters.AddWithValue("@DepartmentId", deptId);
                    cmd.Parameters.AddWithValue("@CustomerId", 0);
                    cmd.Parameters.AddWithValue("@ItemId", 0);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            list.Add(new
                            {
                                itemId = Convert.ToInt32(dr["ItemId"]),
                                item_Code = dr["Item_Code"].ToString(),
                                item_Description = dr["Item_Description"].ToString()
                            });
                        }
                    }
                }
            }

            return list;
        }

        // ── INSERT ──
        public (bool success, string message) Insert(
            ProductionFGIssueModel m, string createdBy)
        {
            if (m.ItemId == 0)
                return (false, "Please select an Item.");
            if (m.Quantity <= 0)
                return (false, "Quantity must be greater than 0.");

            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "FG Issue saved successfully.");
        }

        // ── UPDATE ──
        public void Update(ProductionFGIssueModel m, string updatedBy)
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
                "SP_PRODUCTION_FGISSUE_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            ProductionFGIssueModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "SP_PRODUCTION_FGISSUE_DATA_DETAILS_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            return cmd;
        }

        private ProductionFGIssueModel Map(SqlDataReader dr)
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
            return new ProductionFGIssueModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                ItemId = Val<int>("ItemId"),
                Quantity = Val<int>("Quantity"),
                IsActive = Val<int>("IsActive"),
                CreatedBy = Val<string>("CreatedBy", ""),
                Item_Code = Val<string>("Item_Code", ""),
                Item_Description = Val<string>("Item_Description", ""),
                Department = Val<string>("Department", ""),
                Add_Date = Val<string>("Add_Date", "")
            };
        }
    }
}
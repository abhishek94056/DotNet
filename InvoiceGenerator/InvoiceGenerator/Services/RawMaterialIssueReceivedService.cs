//// Services/RawMaterialIssueReceivedService.cs

//using InvoiceGenerator.Models;
//using Microsoft.Data.SqlClient;
//using System.Data;

//namespace InvoiceGenerator.Services
//{
//    public class RawMaterialIssueReceivedService
//    {
//        private readonly string _conn;

//        public RawMaterialIssueReceivedService(IConfiguration config)
//            => _conn = config.GetConnectionString("InvoiceGenerator")!;

//        // ── SELECT ALL ──
//        public List<RawMaterialIssueReceivedModel> GetAll(int departmentId)
//        {
//            var list = new List<RawMaterialIssueReceivedModel>();

//            using var con = new SqlConnection(_conn);
//            using var cmd = new SqlCommand(
//                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            cmd.Parameters.AddWithValue("@Action", "SelectAll");
//            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);

//            con.Open();

//            using var dr = cmd.ExecuteReader();

//            while (dr.Read())
//                list.Add(MapData(dr));

//            return list;
//        }

//        // ── INSERT ──
//        public void Insert(RawMaterialIssueReceivedModel m, string createdBy)
//        {
//            using var con = new SqlConnection(_conn);
//            using var cmd = BuildCmd("Insert", con, m, createdBy);

//            con.Open();
//            cmd.ExecuteNonQuery();
//        }

//        //Department 

//        public List<DepartmentModel> GetDepartments()
//        {
//            var list = new List<DepartmentModel>();

//            using var con = new SqlConnection(_conn);

//            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con);

//            cmd.CommandType = CommandType.StoredProcedure;

//            cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

//            con.Open();

//            using var dr = cmd.ExecuteReader();

//            while (dr.Read())
//            {
//                list.Add(new DepartmentModel
//                {
//                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
//                    Department = dr["Department"]?.ToString() ?? ""
//                });
//            }

//            return list;
//        }

//        public List<object> GetItemsByDept(int deptId)
//        {
//            var list = new List<object>();

//            using var con = new SqlConnection(_conn);

//            using var cmd = new SqlCommand(
//                "SP_DROP_DOWN_MASTER_GET", con);

//            cmd.CommandType = CommandType.StoredProcedure;

//            cmd.Parameters.AddWithValue("@Action", "SelectAll_ItemByDept");
//            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
//            cmd.Parameters.AddWithValue("@CustomerId", 0);
//            cmd.Parameters.AddWithValue("@ItemId", 0);

//            con.Open();

//            using var dr = cmd.ExecuteReader();

//            while (dr.Read())
//            {
//                list.Add(new
//                {
//                    itemId = Convert.ToInt32(dr["ItemId"]),
//                    item_Code = dr["Item_Code"].ToString(),
//                    item_Description = dr["Item_Description"].ToString()
//                });
//            }

//            return list;
//        }



//        // ── UPDATE ──
//        public void Update(RawMaterialIssueReceivedModel m, string updatedBy)
//        {
//            using var con = new SqlConnection(_conn);
//            using var cmd = BuildCmd("Update", con, m, updatedBy);

//            con.Open();
//            cmd.ExecuteNonQuery();
//        }

//        // ── DELETE ──
//        public void Delete(int srNo)
//        {
//            using var con = new SqlConnection(_conn);

//            using var cmd = new SqlCommand(
//                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            cmd.Parameters.AddWithValue("@Action", "Delete");
//            cmd.Parameters.AddWithValue("@SrNo", srNo);

//            con.Open();
//            cmd.ExecuteNonQuery();
//        }

//        // ── COMMAND BUILDER ──
//        private SqlCommand BuildCmd(
//            string action,
//            SqlConnection con,
//            RawMaterialIssueReceivedModel m,
//            string createdBy)
//        {
//            var cmd = new SqlCommand(
//                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
//            {
//                CommandType = CommandType.StoredProcedure
//            };

//            cmd.Parameters.AddWithValue("@Action", action);
//            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
//            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
//            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
//            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);

//            cmd.Parameters.AddWithValue(
//                "@Date",
//                string.IsNullOrEmpty(m.Date)
//                ? (object)DBNull.Value
//                : DateTime.Parse(m.Date));

//            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
//            cmd.Parameters.AddWithValue("@RM_Status", m.RM_Status);

//            return cmd;
//        }

//        // ── DATA MAPPER ──
//        private RawMaterialIssueReceivedModel MapData(SqlDataReader dr)
//        {
//            T Val<T>(string col, T def = default!)
//            {
//                try
//                {
//                    var v = dr[col];
//                    return v == DBNull.Value
//                        ? def
//                        : (T)Convert.ChangeType(v, typeof(T));
//                }
//                catch
//                {
//                    return def;
//                }
//            }

//            return new RawMaterialIssueReceivedModel
//            {
//                SrNo = Val<int>("SrNo"),
//                DepartmentId = Val<int>("DepartmentId"),
//                SizeId = Val<int>("SizeId"),
//                Issue_Quantity = Val<decimal>("Issue_Quantity"),
//                Received_Quantity = Val<decimal>("Received_Quantity"),
//                IsActive = Val<int>("IsActive"),
//                CreatedBy = Val<string>("CreatedBy", ""),
//                ItemSize_Code = Val<string>("ItemSize_Code", ""),
//                Item_Size = Val<string>("Item_Size", ""),
//                Department = Val<string>("Department", ""),
//                Add_Date = Val<string>("Add_Date", "")
//            };
//        }
//    }
//}

// Services/RawMaterialIssueService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class RawMaterialIssueReceivedService
    {
        private readonly string _conn;

        public RawMaterialIssueReceivedService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        // ── SELECT ALL (current month, by dept) ──
        public List<RawMaterialIssueReceivedModel> GetAll(int deptId)
        {
            var list = new List<RawMaterialIssueReceivedModel>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", deptId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        // ── GET SIZES BY DEPT ──
        public List<object> GetSizesByDept(int deptId)
        {
            var list = new List<object>();
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "SELECT SizeId, ItemSize_Code, Item_Size " +
                "FROM Item_Size_Master " +
                "WHERE DepartmentId = @d AND IsActive = 0 " +
                "ORDER BY ItemSize_Code", con);
            cmd.Parameters.AddWithValue("@d", deptId);
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
        public (bool success, string message) Insert(
            RawMaterialIssueReceivedModel m, string createdBy)
        {
            if (m.SizeId == 0)
                return (false, "Please select an Item Size.");
            if (m.Quantity <= 0)
                return (false, "Quantity must be greater than 0.");
            if (m.RM_Status == 0)
                return (false, "Please select Issue or Received.");

            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Insert", con, m, createdBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, m.RM_Status == 1
                ? "Issue quantity saved successfully."
                : "Received quantity saved successfully.");
        }

        // ── UPDATE ──
        public (bool success, string message) Update(
            RawMaterialIssueReceivedModel m, string updatedBy)
        {
            if (m.Quantity <= 0)
                return (false, "Quantity must be greater than 0.");
            if (m.RM_Status == 0)
                return (false, "Please select Issue or Received.");

            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("Update", con, m, updatedBy);
            con.Open();
            cmd.ExecuteNonQuery();
            return (true, "Record updated successfully.");
        }

        // ── DELETE ──
        public void Delete(int srNo)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand(
                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@SrNo", srNo);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── CMD BUILDER ──
        private SqlCommand BuildCmd(string action, SqlConnection con,
            RawMaterialIssueReceivedModel m, string createdBy)
        {
            var cmd = new SqlCommand(
                "RAW_MATERIAL_ISSUE_RECEIVED_DATA_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", action);
            cmd.Parameters.AddWithValue("@SrNo", m.SrNo);
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@SizeId", m.SizeId);
            cmd.Parameters.AddWithValue("@Quantity", m.Quantity);
            cmd.Parameters.AddWithValue("@Date",
                string.IsNullOrEmpty(m.Date)
                    ? (object)DBNull.Value : DateTime.Parse(m.Date));
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@RM_Status", m.RM_Status);
            return cmd;
        }

        private RawMaterialIssueReceivedModel Map(SqlDataReader dr)
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
            return new RawMaterialIssueReceivedModel
            {
                SrNo = Val<int>("SrNo"),
                DepartmentId = Val<int>("DepartmentId"),
                SizeId = Val<int>("SizeId"),
                Issue_Quantity = Val<decimal>("Issue_Quantity"),
                Received_Quantity = Val<decimal>("Received_Quantity"),
                IsActive = Val<int>("IsActive"),
                CreatedBy = Val<string>("CreatedBy", ""),
                ItemSize_Code = Val<string>("ItemSize_Code", ""),
                Item_Size = Val<string>("Item_Size", ""),
                Department = Val<string>("Department", ""),
                Add_Date = Val<string>("Add_Date", ""),
                Date = Val<string>("Add_Date", "")
            };
        }
    }
}
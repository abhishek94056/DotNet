// Services/ItemDescriptionService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ItemDescriptionPPBoxService
    {
        private readonly string _conn;

        public ItemDescriptionPPBoxService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        public List<ItemDescriptionModel> GetAll(int departmentId)
        {
            var list = new List<ItemDescriptionModel>();
            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET_PP_BOX", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll");
            cmd.Parameters.AddWithValue("@DepartmentId", departmentId);
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) list.Add(Map(dr));
            return list;
        }

        public ItemDescriptionModel? GetById(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET_PP_BOX", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectOne");
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? MapRaw(dr) : null;
        }

        //public List<MachineDescriptionModel> GetMachines()
        //{
        //    var list = new List<MachineDescriptionModel>();
        //    using var con = new SqlConnection(_conn);
        //    //using var cmd = new SqlCommand(
        //    //    "SELECT MachineId, MachineName, DepartmentId FROM MachineMaster ORDER BY MachineName", con);
        //    using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_Machine");
        //    con.Open();
        //    using var dr = cmd.ExecuteReader();
        //    while (dr.Read())
        //        list.Add(new MachineDescriptionModel
        //        {
        //            MachineId = Convert.ToInt32(dr["MachineId"]),
        //            MachineName = dr["MachineName"].ToString()!,
        //            DepartmentId = dr["DepartmentId"] == DBNull.Value
        //                ? 0 : Convert.ToInt32(dr["DepartmentId"])
        //        });
        //    return list;
        //}

        public List<PackingModel> GetPackingTypes()
        {
            var list = new List<PackingModel>();
            using var con = new SqlConnection(_conn);
            //using var cmd = new SqlCommand(
            //    "SELECT PackingId, Packing_Type, DepartmentId FROM Packing_Type_Master ORDER BY Packing_Type", con);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_PackingType");
            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new PackingModel
                {
                    PackingId = Convert.ToInt32(dr["PackingId"]),
                    Packing_Type = dr["Packing_Type"].ToString(),
                    DepartmentId = dr["DepartmentId"] == DBNull.Value
                        ? 0 : Convert.ToInt32(dr["DepartmentId"])
                });
            }
            return list;
        }
        public List<DepartmentModel> GetDepartments()
        {
            var list = new List<DepartmentModel>();

            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            {
                CommandType = CommandType.StoredProcedure
            };

            cmd.Parameters.AddWithValue("@Action", "SelectAll_Department");

            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new DepartmentModel
                {
                    DepartmentId = Convert.ToInt32(dr["DepartmentId"]),
                    Department = dr["Department"].ToString()
                });
            }

            return list;
        }
        //public List<Inner_PackingModel> GetInnerPackingTypes()
        //{
        //    var list = new List<Inner_PackingModel>();
        //    using var con = new SqlConnection(_conn);
        //    //using var cmd = new SqlCommand(
        //    //    "SELECT Inner_PackingId, Inner_Packing_Type, DepartmentId FROM Packing_Type_Polybag_Inner_Master ORDER BY Inner_Packing_Type", con);
        //    using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
        //    { CommandType = CommandType.StoredProcedure };
        //    cmd.Parameters.AddWithValue("@Action", "SelectAll_Inner_PackingType");

        //    con.Open();
        //    using var dr = cmd.ExecuteReader();

        //    while (dr.Read())
        //    {
        //        list.Add(new Inner_PackingModel
        //        {
        //            Inner_PackingId = Convert.ToInt32(dr["Inner_PackingId"]),
        //            Inner_Packing_Type = dr["Inner_Packing_Type"].ToString(),
        //            DepartmentId = dr["DepartmentId"] == DBNull.Value
        //                ? 0 : Convert.ToInt32(dr["DepartmentId"])
        //        });
        //    }
        //    return list;
        //}

        public (bool success, string message, int id) Insert(
            ItemDescriptionModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("SP_ITEM_MASTER_SET_PP_BOX", con, m, createdBy);
            cmd.Parameters.AddWithValue("@Action", "Insert");
            con.Open();
            //int newId = Convert.ToInt32(cmd.ExecuteScalar());
            //if (newId == -1)
            //int rows = cmd.ExecuteNonQuery();
            //if (rows == 0)
            //    return (false, "Item Code already exists.", 0);
            //return (true, "Item saved successfully.", rows);
            int rows = cmd.ExecuteNonQuery();   // ✅ CORRECT

            if (rows > 0)
                return (true, "Item saved successfully.", 0);

            // ⚠️ Could be duplicate OR failure
            return (false, "Item already exists.", 0);
        }

        public void Update(ItemDescriptionModel m, string updatedBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("SP_ITEM_MASTER_SET_PP_BOX", con, m, updatedBy);
            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET_PP_BOX", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── Shared parameter builder ──
        private SqlCommand BuildCmd(string sp, SqlConnection con,
    ItemDescriptionModel m, string createdBy)
        {
            var cmd = new SqlCommand(sp, con)
            {
                CommandType = CommandType.StoredProcedure
            };

            // Basic
            cmd.Parameters.AddWithValue("@Item_Code", m.Item_Code);
            cmd.Parameters.AddWithValue("@Item_Description", m.Item_Description);

            // Rates
            cmd.Parameters.AddWithValue("@MRP_Rate_RM", m.MRP_Rate_RM);
            cmd.Parameters.AddWithValue("@MRP_Rate_Sale", m.MRP_Rate_Sale);

            // RM Sizes + Sheet Qty
            cmd.Parameters.AddWithValue("@RM_SizeId_1", (object?)m.RM_SizeId_1 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sheet_Qty_1", m.Sheet_Qty_1);

            cmd.Parameters.AddWithValue("@RM_SizeId_2", (object?)m.RM_SizeId_2 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sheet_Qty_2", m.Sheet_Qty_2);

            cmd.Parameters.AddWithValue("@RM_SizeId_3", (object?)m.RM_SizeId_3 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sheet_Qty_3", m.Sheet_Qty_3);

            cmd.Parameters.AddWithValue("@RM_SizeId_4", (object?)m.RM_SizeId_4 ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Sheet_Qty_4", m.Sheet_Qty_4);

            // Department + User
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);

            // Stock
            cmd.Parameters.AddWithValue("@Opening_Stock", m.Opening_Stock);
            cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
            cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);

            // Packing
            cmd.Parameters.AddWithValue("@Packing_Type",
                string.IsNullOrEmpty(m.Packing_Type) ? (object)DBNull.Value : m.Packing_Type);

            cmd.Parameters.AddWithValue("@Packing_Qty", m.Packing_Qty);

            // Sale cost
            cmd.Parameters.AddWithValue("@Sale_Trans_Cost", m.Sale_Trans_Cost);

            // Inner Packing
            cmd.Parameters.AddWithValue("@Inner_Packing_Type",
                string.IsNullOrEmpty(m.Inner_Packing_Type) ? (object)DBNull.Value : m.Inner_Packing_Type);

            cmd.Parameters.AddWithValue("@Inner_Packing_Qty", m.Inner_Packing_Qty);

            return cmd;
        }
        private ItemDescriptionModel Map(SqlDataReader dr) => new()
        {
            ItemId = Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,

            MRP_Rate_RM = dr["MRP_Rate_RM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_RM"]),
            MRP_Rate_Sale = dr["MRP_Rate_Sale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_Sale"]),

            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
            Department = dr["Department"].ToString()!,

            CreatedBy = dr["CreatedBy"].ToString()!,
            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAX_Stock"]),
            Opening_Stock = dr["Opening_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Opening_Stock"]),

            // RM Sizes
            RM_SizeId_1 = dr["RM_SizeId_1"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_1"]),
            Sheet_Qty_1 = dr["Sheet_Qty_1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_1"]),

            RM_SizeId_2 = dr["RM_SizeId_2"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_2"]),
            Sheet_Qty_2 = dr["Sheet_Qty_2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_2"]),

            RM_SizeId_3 = dr["RM_SizeId_3"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_3"]),
            Sheet_Qty_3 = dr["Sheet_Qty_3"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_3"]),

            RM_SizeId_4 = dr["RM_SizeId_4"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_4"]),
            Sheet_Qty_4 = dr["Sheet_Qty_4"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_4"]),

            // Size Codes (from join)
            ItemSize_Code_1 = dr["ItemSize_Code_1"].ToString()!,
            ItemSize_Code_2 = dr["ItemSize_Code_2"].ToString()!,
            ItemSize_Code_3 = dr["ItemSize_Code_3"].ToString()!,
            ItemSize_Code_4 = dr["ItemSize_Code_4"].ToString()!,

            // Packing
            Packing_Type = dr["Packing_Type"].ToString()!,
            Packing_Qty = dr["Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Packing_Qty"]),

            // Cost
            Sale_Trans_Cost = dr["Sale_Trans_Cost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Sale_Trans_Cost"]),

            // Inner Packing
            Inner_Packing_Type = dr["Inner_Packing_Type"].ToString()!,
            Inner_Packing_Qty = dr["Inner_Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Inner_Packing_Qty"])
        };

        private ItemDescriptionModel MapRaw(SqlDataReader dr) => new()
        {
            ItemId = Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,

            MRP_Rate_RM = dr["MRP_Rate_RM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_RM"]),
            MRP_Rate_Sale = dr["MRP_Rate_Sale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_Sale"]),

            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),

            CreatedBy = dr["CreatedBy"].ToString()!,
            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAX_Stock"]),
            Opening_Stock = dr["Opening_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Opening_Stock"]),

            // RM Sizes
            RM_SizeId_1 = dr["RM_SizeId_1"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_1"]),
            Sheet_Qty_1 = dr["Sheet_Qty_1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_1"]),

            RM_SizeId_2 = dr["RM_SizeId_2"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_2"]),
            Sheet_Qty_2 = dr["Sheet_Qty_2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_2"]),

            RM_SizeId_3 = dr["RM_SizeId_3"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_3"]),
            Sheet_Qty_3 = dr["Sheet_Qty_3"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_3"]),

            RM_SizeId_4 = dr["RM_SizeId_4"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_4"]),
            Sheet_Qty_4 = dr["Sheet_Qty_4"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_4"]),

            // Packing
            Packing_Type = dr["Packing_Type"].ToString()!,
            Packing_Qty = dr["Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Packing_Qty"]),

            // Cost
            Sale_Trans_Cost = dr["Sale_Trans_Cost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Sale_Trans_Cost"]),

            // Inner Packing
            Inner_Packing_Type = dr["Inner_Packing_Type"].ToString()!,
            Inner_Packing_Qty = dr["Inner_Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Inner_Packing_Qty"])
        };
    }
}
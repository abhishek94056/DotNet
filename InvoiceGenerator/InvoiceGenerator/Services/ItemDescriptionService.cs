// Services/ItemDescriptionService.cs
using InvoiceGenerator.Models;
using Microsoft.Data.SqlClient;
using System.Data;

namespace InvoiceGenerator.Services
{
    public class ItemDescriptionService
    {
        private readonly string _conn;

        public ItemDescriptionService(IConfiguration config)
            => _conn = config.GetConnectionString("InvoiceGenerator")!;

        public List<ItemDescriptionModel> GetAll(int departmentId)
        {
            var list = new List<ItemDescriptionModel>();
            using var con = new SqlConnection(_conn);

            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET", con)
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
            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectOne");
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? MapRaw(dr) : null;
        }

        public List<MachineDescriptionModel> GetMachines()
        {
            var list = new List<MachineDescriptionModel>();
            using var con = new SqlConnection(_conn);
            //using var cmd = new SqlCommand(
            //    "SELECT MachineId, MachineName, DepartmentId FROM MachineMaster ORDER BY MachineName", con);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Machine");
            con.Open();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                list.Add(new MachineDescriptionModel
                {
                    MachineId = Convert.ToInt32(dr["MachineId"]),
                    MachineName = dr["MachineName"].ToString()!,
                    DepartmentId = dr["DepartmentId"] == DBNull.Value
                        ? 0 : Convert.ToInt32(dr["DepartmentId"])
                });
            return list;
        }

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

        public List<Inner_PackingModel> GetInnerPackingTypes()
        {
            var list = new List<Inner_PackingModel>();
            using var con = new SqlConnection(_conn);
            //using var cmd = new SqlCommand(
            //    "SELECT Inner_PackingId, Inner_Packing_Type, DepartmentId FROM Packing_Type_Polybag_Inner_Master ORDER BY Inner_Packing_Type", con);
            using var cmd = new SqlCommand("SP_DROP_DOWN_MASTER_GET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "SelectAll_Inner_PackingType");

            con.Open();
            using var dr = cmd.ExecuteReader();

            while (dr.Read())
            {
                list.Add(new Inner_PackingModel
                {
                    Inner_PackingId = Convert.ToInt32(dr["Inner_PackingId"]),
                    Inner_Packing_Type = dr["Inner_Packing_Type"].ToString(),
                    DepartmentId = dr["DepartmentId"] == DBNull.Value
                        ? 0 : Convert.ToInt32(dr["DepartmentId"])
                });
            }
            return list;
        }

        public (bool success, string message, int id) Insert(
            ItemDescriptionModel m, string createdBy)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = BuildCmd("SP_ITEM_MASTER_SET", con, m, createdBy);
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
            using var cmd = BuildCmd("SP_ITEM_MASTER_SET", con, m, updatedBy);
            cmd.Parameters.AddWithValue("@Action", "Update");
            cmd.Parameters.AddWithValue("@ItemId", m.ItemId);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        public void Delete(int id)
        {
            using var con = new SqlConnection(_conn);
            using var cmd = new SqlCommand("SP_ITEM_MASTER_SET", con)
            { CommandType = CommandType.StoredProcedure };
            cmd.Parameters.AddWithValue("@Action", "Delete");
            cmd.Parameters.AddWithValue("@ItemId", id);
            con.Open();
            cmd.ExecuteNonQuery();
        }

        // ── Shared parameter builder ──
        //private SqlCommand BuildCmd(string sp, SqlConnection con,
        //    ItemDescriptionModel m, string createdBy)
        //{
        //    var cmd = new SqlCommand(sp, con)
        //    { CommandType = CommandType.StoredProcedure };

        //    cmd.Parameters.AddWithValue("@Item_Code", m.Item_Code);
        //    cmd.Parameters.AddWithValue("@Item_Description", m.Item_Description);
        //    cmd.Parameters.AddWithValue("@Cycle_Time", m.Cycle_Time);
        //    cmd.Parameters.AddWithValue("@No_of_Cavity", m.No_of_Cavity);
        //    cmd.Parameters.AddWithValue("@Std_Shot_Weight", m.Std_Shot_Weight);
        //    cmd.Parameters.AddWithValue("@Finish_Weight", m.Finish_Weight);
        //    cmd.Parameters.AddWithValue("@MRP_Rate_RM", m.MRP_Rate_RM);
        //    cmd.Parameters.AddWithValue("@MRP_Rate_Sale", m.MRP_Rate_Sale);
        //    cmd.Parameters.AddWithValue("@Ext_Mould_PVC_RMType", m.Ext_Mould_PVC_RMType ?? "");
        //    cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
        //    cmd.Parameters.AddWithValue("@MachineId",m.MachineId == 0 ? (object)DBNull.Value : m.MachineId);
        //    cmd.Parameters.AddWithValue("@Date", m.Date);
        //    cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
        //    cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
        //    cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);
        //    cmd.Parameters.AddWithValue("@Opening_Stock", m.Opening_Stock);
        //    cmd.Parameters.AddWithValue("@RM_SizeId_1", (object?)m.RM_SizeId_1 ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@Sheet_Qty_1", m.Sheet_Qty_1);
        //    cmd.Parameters.AddWithValue("@RM_SizeId_2", (object?)m.RM_SizeId_2 ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@Sheet_Qty_2", m.Sheet_Qty_2);
        //    cmd.Parameters.AddWithValue("@RM_SizeId_3", (object?)m.RM_SizeId_3 ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@Sheet_Qty_3", m.Sheet_Qty_3);
        //    cmd.Parameters.AddWithValue("@RM_SizeId_4", (object?)m.RM_SizeId_4 ?? DBNull.Value);
        //    cmd.Parameters.AddWithValue("@Sheet_Qty_4", m.Sheet_Qty_4);
        //    cmd.Parameters.AddWithValue("@PackingId", m.PackingId == null ? (object)DBNull.Value : m.PackingId);
        //    cmd.Parameters.AddWithValue("@Packing_Qty", m.Packing_Qty);
        //    cmd.Parameters.AddWithValue("@Lab", m.Lab);
        //    cmd.Parameters.AddWithValue("@Remark_Std_Shot_Weight", m.Remark_Std_Shot_Weight ?? "");
        //    cmd.Parameters.AddWithValue("@Remark_Finish_Weight", m.Remark_Finish_Weight ?? "");
        //    cmd.Parameters.AddWithValue("@Remark_MRP_Rate_RM", m.Remark_MRP_Rate_RM ?? "");
        //    cmd.Parameters.AddWithValue("@Remark_MRP_Rate_Sale", m.Remark_MRP_Rate_Sale ?? "");
        //    cmd.Parameters.AddWithValue("@Sale_Trans_Cost", m.Sale_Trans_Cost);
        //    cmd.Parameters.AddWithValue("@Inner_PackingId", m.Inner_PackingId == null ? (object)DBNull.Value : m.Inner_PackingId);
        //    cmd.Parameters.AddWithValue("@Inner_Packing_Qty", m.Inner_Packing_Qty);
        //    return cmd;
        //}
        private SqlCommand BuildCmd(string sp, SqlConnection con,
    ItemDescriptionModel m, string createdBy)
        {
            var cmd = new SqlCommand(sp, con)
            { CommandType = CommandType.StoredProcedure };

            cmd.Parameters.AddWithValue("@Item_Code", m.Item_Code);
            cmd.Parameters.AddWithValue("@Item_Description", m.Item_Description);
            cmd.Parameters.AddWithValue("@Cycle_Time", m.Cycle_Time);
            cmd.Parameters.AddWithValue("@No_of_Cavity", m.No_of_Cavity);
            cmd.Parameters.AddWithValue("@Std_Shot_Weight", m.Std_Shot_Weight);
            cmd.Parameters.AddWithValue("@Finish_Weight", m.Finish_Weight);
            cmd.Parameters.AddWithValue("@MRP_Rate_RM", m.MRP_Rate_RM);
            cmd.Parameters.AddWithValue("@MRP_Rate_Sale", m.MRP_Rate_Sale);
            cmd.Parameters.AddWithValue("@Ext_Mould_PVC_RMType", m.Ext_Mould_PVC_RMType ?? "");
            cmd.Parameters.AddWithValue("@DepartmentId", m.DepartmentId);
            cmd.Parameters.AddWithValue("@MachineId", m.MachineId);
            cmd.Parameters.AddWithValue("@CreatedBy", createdBy);
            cmd.Parameters.AddWithValue("@MIN_Stock", m.MIN_Stock);
            cmd.Parameters.AddWithValue("@MAX_Stock", m.MAX_Stock);
            cmd.Parameters.AddWithValue("@Opening_Stock", m.Opening_Stock);

            // 🔥 IMPORTANT: change these
            cmd.Parameters.AddWithValue("@Packing_Type", m.Packing_Type);
            cmd.Parameters.AddWithValue("@Packing_Qty", m.Packing_Qty);

            cmd.Parameters.AddWithValue("@Remark_Std_Shot_Weight", m.Remark_Std_Shot_Weight ?? "");
            cmd.Parameters.AddWithValue("@Remark_Finish_Weight", m.Remark_Finish_Weight ?? "");
            cmd.Parameters.AddWithValue("@Remark_MRP_Rate_RM", m.Remark_MRP_Rate_RM ?? "");
            cmd.Parameters.AddWithValue("@Remark_MRP_Rate_Sale", m.Remark_MRP_Rate_Sale ?? "");
            cmd.Parameters.AddWithValue("@Sale_Trans_Cost", m.Sale_Trans_Cost);

            // 🔥 IMPORTANT
            cmd.Parameters.AddWithValue("@Inner_Packing_Type", m.Inner_Packing_Type);
            cmd.Parameters.AddWithValue("@Inner_Packing_Qty", m.Inner_Packing_Qty);

            return cmd;
        }
        private ItemDescriptionModel Map(SqlDataReader dr) => new()
        {
            ItemId = Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,
            Cycle_Time = dr["Cycle_Time"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Cycle_Time"]),
            No_of_Cavity = dr["No_of_Cavity"] == DBNull.Value ? 0 : Convert.ToInt32(dr["No_of_Cavity"]),
            Std_Shot_Weight = dr["Std_Shot_Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Std_Shot_Weight"]),
            Finish_Weight = dr["Finish_Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Finish_Weight"]),
            MRP_Rate_RM = dr["MRP_Rate_RM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_RM"]),
            MRP_Rate_Sale = dr["MRP_Rate_Sale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_Sale"]),
            Ext_Mould_PVC_RMType = dr["Ext_Mould_PVC_RMType"].ToString()!,
            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
            Department = dr["Department"].ToString()!,

            MachineId = dr["MachineId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MachineId"]),
            MachineName = dr["MachineName"].ToString()!,
            Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
            CreatedBy = dr["CreatedBy"].ToString()!,
            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAX_Stock"]),
            Opening_Stock = dr["Opening_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Opening_Stock"]),
            RM_SizeId_1 = dr["RM_SizeId_1"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_1"]),
            Sheet_Qty_1 = dr["Sheet_Qty_1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_1"]),
            RM_SizeId_2 = dr["RM_SizeId_2"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_2"]),
            Sheet_Qty_2 = dr["Sheet_Qty_2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_2"]),
            RM_SizeId_3 = dr["RM_SizeId_3"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_3"]),
            Sheet_Qty_3 = dr["Sheet_Qty_3"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_3"]),
            RM_SizeId_4 = dr["RM_SizeId_4"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_4"]),
            Sheet_Qty_4 = dr["Sheet_Qty_4"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_4"]),
            //SizeName_1 = dr["SizeName_1"].ToString()!,
            //SizeName_2 = dr["SizeName_2"].ToString()!,
            //SizeName_3 = dr["SizeName_3"].ToString()!,
            //SizeName_4 = dr["SizeName_4"].ToString()!,        
            //PackingId = dr["PackingId"] == DBNull.Value ? null : Convert.ToInt32(dr["PackingId"]),
            Packing_Type = dr["Packing_Type"].ToString()!,
            Packing_Qty = dr["Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Packing_Qty"]),
            Lab = dr["Lab"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Lab"]),
            Remark_Std_Shot_Weight = dr["Remark_Std_Shot_Weight"].ToString()!,
            Remark_Finish_Weight = dr["Remark_Finish_Weight"].ToString()!,
            Remark_MRP_Rate_RM = dr["Remark_MRP_Rate_RM"].ToString()!,
            Remark_MRP_Rate_Sale = dr["Remark_MRP_Rate_Sale"].ToString()!,
            Sale_Trans_Cost = dr["Sale_Trans_Cost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Sale_Trans_Cost"]),
            //Inner_PackingId = dr["Inner_PackingId"] == DBNull.Value ? null : Convert.ToInt32(dr["Inner_PackingId"]),
            Inner_Packing_Type = dr["Inner_Packing_Type"].ToString()!,
            Inner_Packing_Qty = dr["Inner_Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Inner_Packing_Qty"])
        };

        private ItemDescriptionModel MapRaw(SqlDataReader dr) => new()
        {
            ItemId = Convert.ToInt32(dr["ItemId"]),
            Item_Code = dr["Item_Code"].ToString()!,
            Item_Description = dr["Item_Description"].ToString()!,
            Cycle_Time = dr["Cycle_Time"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Cycle_Time"]),
            No_of_Cavity = dr["No_of_Cavity"] == DBNull.Value ? 0 : Convert.ToInt32(dr["No_of_Cavity"]),
            Std_Shot_Weight = dr["Std_Shot_Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Std_Shot_Weight"]),
            Finish_Weight = dr["Finish_Weight"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Finish_Weight"]),
            MRP_Rate_RM = dr["MRP_Rate_RM"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_RM"]),
            MRP_Rate_Sale = dr["MRP_Rate_Sale"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["MRP_Rate_Sale"]),
            Ext_Mould_PVC_RMType = dr["Ext_Mould_PVC_RMType"].ToString()!,
            DepartmentId = dr["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["DepartmentId"]),
            MachineId = dr["MachineId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MachineId"]),
            Date = dr["Date"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["Date"]),
            CreatedBy = dr["CreatedBy"].ToString()!,
            MIN_Stock = dr["MIN_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MIN_Stock"]),
            MAX_Stock = dr["MAX_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["MAX_Stock"]),
            Opening_Stock = dr["Opening_Stock"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Opening_Stock"]),
            RM_SizeId_1 = dr["RM_SizeId_1"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_1"]),
            Sheet_Qty_1 = dr["Sheet_Qty_1"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_1"]),
            RM_SizeId_2 = dr["RM_SizeId_2"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_2"]),
            Sheet_Qty_2 = dr["Sheet_Qty_2"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_2"]),
            RM_SizeId_3 = dr["RM_SizeId_3"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_3"]),
            Sheet_Qty_3 = dr["Sheet_Qty_3"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_3"]),
            RM_SizeId_4 = dr["RM_SizeId_4"] == DBNull.Value ? null : Convert.ToInt32(dr["RM_SizeId_4"]),
            Sheet_Qty_4 = dr["Sheet_Qty_4"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Sheet_Qty_4"]),
            //PackingId = dr["PackingId"] == DBNull.Value ? null : Convert.ToInt32(dr["PackingId"]),
            
            Packing_Type = dr["Packing_Type"].ToString()!,
            Packing_Qty = dr["Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Packing_Qty"]),
            Lab = dr["Lab"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Lab"]),
            Remark_Std_Shot_Weight = dr["Remark_Std_Shot_Weight"].ToString()!,
            Remark_Finish_Weight = dr["Remark_Finish_Weight"].ToString()!,
            Remark_MRP_Rate_RM = dr["Remark_MRP_Rate_RM"].ToString()!,
            Remark_MRP_Rate_Sale = dr["Remark_MRP_Rate_Sale"].ToString()!,
            Sale_Trans_Cost = dr["Sale_Trans_Cost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Sale_Trans_Cost"]),
            //Inner_PackingId = dr["Inner_PackingId"] == DBNull.Value ? null : Convert.ToInt32(dr["Inner_PackingId"]),
            Inner_Packing_Type = dr["Inner_Packing_Type"].ToString()!,
            Inner_Packing_Qty = dr["Inner_Packing_Qty"] == DBNull.Value ? 0 : Convert.ToInt32(dr["Inner_Packing_Qty"])
        };
    }
}
// Models/ProductionPlanModel.cs
namespace InvoiceGenerator.Models
{
    public class ProductionPlanModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int MachineId { get; set; }
        public int ItemId { get; set; }
        public int Plan_Qty { get; set; }
        public int SizeId { get; set; }
        public string Plan_Date { get; set; } = "";
        public string Remark { get; set; } = "";
        public int DepartmentId { get; set; }
        public string CreatedBy { get; set; } = "";

        // ── Display (from JOINs / functions) ──
        public string MachineName { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string Add_Date { get; set; } = "";
        public string ItemSize_Code { get; set; } = "";
        public decimal RM_Required { get; set; }
        public int Produce_Qty { get; set; }
        public int IsActive { get; set; }
    }

    // ── For RM stock info ──
    public class RMStockInfoModel
    {
        public decimal Stock_RM_Qty { get; set; }
        public decimal Used_RM_Qty { get; set; }
        public decimal Available_RM_Qty { get; set; }
        public decimal Shot_Weight { get; set; }
    }

    // ── For validation panels ──
    public class ProductionValidateModel
    {
        public decimal Required_FinalTime_InHrs { get; set; }
    }

    public class POScheduleQtyModel
    {
        public int POSchedule_Qty { get; set; }
    }

    public class AddedPlanQtyModel
    {
        public int AddedPlan_Qty { get; set; }
    }

    public class FGQtyModel
    {
        public int FG_Qty { get; set; }
    }
}
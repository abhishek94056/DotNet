// Models/MachineProductionModel.cs
namespace InvoiceGenerator.Models
{
    public class MachineProductionModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int ShiftId { get; set; }
        public int MachineId { get; set; }
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public string Date { get; set; } = "";
        public int DepartmentId { get; set; }
        public string CreatedBy { get; set; } = "";
        public int SizeId { get; set; }
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string ShiftName { get; set; } = "";
        public string MachineName { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string Add_Date { get; set; } = "";
    }

    public class ProduceQtyModel
    {
        public int Produce_Qty { get; set; }
    }

    public class PlanQtyModel
    {
        public int AddedPlan_Qty { get; set; }
    }
}
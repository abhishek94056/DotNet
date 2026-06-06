// Models/MachineRejectionModel.cs
namespace InvoiceGenerator.Models
{
    public class MachineRejectionModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int ShiftId { get; set; }
        public int MachineId { get; set; }
        public int ItemId { get; set; }
        public int OperatorId { get; set; }
        public int RejectionId { get; set; }
        public int Rejection_Qty { get; set; }
        public string Date { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int DepartmentId { get; set; }
        public decimal Actual_Shot_Weight { get; set; }
        public string Remark { get; set; } = "";
        public decimal Finish_Weight { get; set; }

        // ── Display (from JOINs) ──
        public string ShiftName { get; set; } = "";
        public string MachineName { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string DepartmentName { get; set; } = "";
        public string Rejection_Reason { get; set; } = "";
        public string OperatorName { get; set; } = "";
        public string Add_Date { get; set; } = "";
    }
}
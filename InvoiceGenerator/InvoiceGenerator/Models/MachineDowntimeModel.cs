// Models/MachineDowntimeModel.cs
namespace InvoiceGenerator.Models
{
    public class MachineDowntimeModel
    {
        // ── Editable ──
        public int SrNo { get; set; }
        public int DepartmentId { get; set; }
        public int ShiftId { get; set; }
        public int MachineId { get; set; }
        public int ItemId { get; set; }
        public int MC_StatusId { get; set; }
        public int DownTime_ReasonId { get; set; }
        public decimal DownTime { get; set; }
        public string Date { get; set; } = "";
        public string CreatedBy { get; set; } = "";
        public int IsActive { get; set; }

        // ── Display (from JOINs) ──
        public string Department { get; set; } = "";
        public string ShiftName { get; set; } = "";
        public string MachineName { get; set; } = "";
        public string Item_Code { get; set; } = "";
        public string Item_Description { get; set; } = "";
        public string Add_Date { get; set; } = "";

        // ── Downtime columns (from table) ──
        public decimal Actual_Production_Hrs { get; set; }
        public decimal No_Operator_InHrs { get; set; }
        public decimal Tool_Change_InHrs { get; set; }
        public decimal No_Power_InHrs { get; set; }
        public decimal Machine_Break_Down_InHrs { get; set; }
        public decimal No_Material_InHrs { get; set; }
        public decimal MC_Setting_InHrs { get; set; }
        public decimal No_Load_InHrs { get; set; }
        public decimal Training_InHrs { get; set; }
        public decimal QualityIssue_InHrs { get; set; }
    }
}